//! A C ABI over `concerto-core`, the Rust Concerto engine, for the .NET
//! binding (`AccordProject.Concerto.Engine`).
//!
//! This plays the role `concerto-wasm` plays for `@accordproject/concerto-core`:
//! a thin boundary layer, with the engine doing all the work. The boundary
//! contract is deliberately small:
//!
//! - A `ModelManager` crosses the boundary as an opaque pointer, created by
//!   [`concerto_model_manager_new`] and released by
//!   [`concerto_model_manager_free`] (a `SafeHandle` on the .NET side).
//! - Every other input is a NUL-terminated UTF-8 JSON string.
//! - Every fallible call returns a NUL-terminated UTF-8 JSON *envelope*,
//!   owned by the caller and released with [`concerto_string_free`]:
//!   `{"ok":true,"value":...}` or `{"ok":false,"error":{...}}`. The error
//!   carries the engine's stable `kind` and `code` (docs/public-api.md
//!   section 2), which the .NET side maps to `ConcertoException`.
//! - No panic crosses the boundary: each entry point runs under
//!   `catch_unwind` and reports a panic as an `Internal` error.

use std::ffi::{CStr, CString, c_char};
use std::panic::{AssertUnwindSafe, catch_unwind};

use concerto_core::instance::{Diagnostic, ValidationOptions};
use concerto_core::{Error, ModelManager, json};
use serde::{Deserialize, Serialize};
use serde_json::json as sjson;

/// The opaque handle the .NET side holds.
pub struct ConcertoModelManager(ModelManager);

// ---------------------------------------------------------------------------
// Envelope helpers
// ---------------------------------------------------------------------------

#[derive(Serialize)]
struct ErrorPayload {
    /// The engine's `ErrorKind` (`IllegalModel`, `Validation`, ...), or
    /// `InvalidArgument` / `Internal` for boundary failures.
    kind: String,
    /// The message catalogue key: stable, safe to match on.
    code: String,
    message: String,
    #[serde(skip_serializing_if = "Option::is_none")]
    file_name: Option<String>,
    /// The accordproject/concerto#1273 details of a strict-mode rejection.
    #[serde(skip_serializing_if = "Vec::is_empty")]
    details: Vec<DetailPayload>,
}

#[derive(Serialize)]
struct DetailPayload {
    path: String,
    code: &'static str,
    #[serde(skip_serializing_if = "Option::is_none")]
    expected: Option<String>,
    #[serde(skip_serializing_if = "Option::is_none")]
    actual: Option<String>,
}

impl From<&Error> for ErrorPayload {
    fn from(err: &Error) -> Self {
        Self {
            kind: format!("{:?}", err.kind()),
            code: err.code().to_string(),
            message: err.to_string(),
            file_name: err.file_name().map(str::to_string),
            details: err
                .details()
                .iter()
                .map(|d| DetailPayload {
                    path: d.path.clone(),
                    code: d.code.as_str(),
                    expected: d.expected.clone(),
                    actual: d.actual.clone(),
                })
                .collect(),
        }
    }
}

impl ErrorPayload {
    fn boundary(kind: &str, code: &str, message: impl Into<String>) -> Self {
        Self {
            kind: kind.to_string(),
            code: code.to_string(),
            message: message.into(),
            file_name: None,
            details: Vec::new(),
        }
    }
}

type FfiResult = Result<serde_json::Value, ErrorPayload>;

fn into_c_string(text: String) -> *mut c_char {
    // serde_json never emits an interior NUL (it escapes U+0000), so this
    // cannot fail; fall back to a fixed envelope rather than panic.
    CString::new(text)
        .unwrap_or_else(|_| {
            CString::from(c"{\"ok\":false,\"error\":{\"kind\":\"Internal\",\"code\":\"ffi-nul\",\"message\":\"interior NUL\"}}")
        })
        .into_raw()
}

/// Runs `body` with panics caught, and wraps its outcome in an envelope.
fn envelope(body: impl FnOnce() -> FfiResult) -> *mut c_char {
    let outcome = catch_unwind(AssertUnwindSafe(body)).unwrap_or_else(|panic| {
        let message = panic
            .downcast_ref::<&str>()
            .map(|s| (*s).to_string())
            .or_else(|| panic.downcast_ref::<String>().cloned())
            .unwrap_or_else(|| "the engine panicked".to_string());
        Err(ErrorPayload::boundary("Internal", "ffi-panic", message))
    });
    let value = match outcome {
        Ok(value) => sjson!({ "ok": true, "value": value }),
        Err(error) => sjson!({ "ok": false, "error": error }),
    };
    into_c_string(value.to_string())
}

/// Reads a NUL-terminated UTF-8 argument.
///
/// # Safety
/// `ptr` must be null or point to a NUL-terminated string that outlives the call.
unsafe fn arg<'a>(ptr: *const c_char, name: &str) -> Result<&'a str, ErrorPayload> {
    if ptr.is_null() {
        return Err(ErrorPayload::boundary(
            "InvalidArgument",
            "ffi-null-argument",
            format!("`{name}` is null"),
        ));
    }
    // SAFETY: the caller guarantees a valid NUL-terminated string.
    unsafe { CStr::from_ptr(ptr) }.to_str().map_err(|_| {
        ErrorPayload::boundary(
            "InvalidArgument",
            "ffi-invalid-utf8",
            format!("`{name}` is not UTF-8"),
        )
    })
}

/// Parses JSON text into the engine's own (key-order preserving) value.
fn parse_json(text: &str, name: &str) -> Result<json::Value, ErrorPayload> {
    serde_json::from_str(text).map_err(|e| {
        ErrorPayload::boundary(
            "InvalidArgument",
            "ffi-invalid-json",
            format!("`{name}` is not valid JSON: {e}"),
        )
    })
}

/// # Safety
/// `ptr` must be null or a live pointer from [`concerto_model_manager_new`].
unsafe fn manager<'a>(ptr: *const ConcertoModelManager) -> Result<&'a ModelManager, ErrorPayload> {
    // SAFETY: the caller guarantees `ptr` is null or live.
    unsafe { ptr.as_ref() }.map(|m| &m.0).ok_or_else(|| {
        ErrorPayload::boundary(
            "InvalidArgument",
            "ffi-null-handle",
            "the model manager handle is null",
        )
    })
}

// ---------------------------------------------------------------------------
// Options and diagnostics
// ---------------------------------------------------------------------------

/// The .NET `ValidationOptions`, as camelCase JSON. Every field is optional.
#[derive(Deserialize, Default)]
#[serde(rename_all = "camelCase", default)]
struct OptionsPayload {
    strict: bool,
    reject_unknown_keys: Option<bool>,
    reject_required_null: Option<bool>,
    permit_resources_for_relationships: bool,
    convert_resources_to_relationships: bool,
}

impl OptionsPayload {
    fn into_options(self) -> ValidationOptions {
        let mut options = if self.strict {
            ValidationOptions::STRICT
        } else {
            ValidationOptions::default()
        };
        if let Some(v) = self.reject_unknown_keys {
            options.reject_unknown_keys = v;
        }
        if let Some(v) = self.reject_required_null {
            options.reject_required_null = v;
        }
        options.permit_resources_for_relationships = self.permit_resources_for_relationships;
        options.convert_resources_to_relationships = self.convert_resources_to_relationships;
        options
    }
}

unsafe fn options(ptr: *const c_char) -> Result<ValidationOptions, ErrorPayload> {
    if ptr.is_null() {
        return Ok(ValidationOptions::default());
    }
    // SAFETY: forwarded from the caller.
    let text = unsafe { arg(ptr, "options") }?;
    serde_json::from_str::<OptionsPayload>(text)
        .map(OptionsPayload::into_options)
        .map_err(|e| {
            ErrorPayload::boundary(
                "InvalidArgument",
                "ffi-invalid-options",
                format!("`options` is not valid: {e}"),
            )
        })
}

fn diagnostic_json(d: &Diagnostic) -> serde_json::Value {
    sjson!({
        "pointer": d.pointer,
        "code": d.code.as_str(),
        "severity": format!("{:?}", d.severity),
        "message": d.message,
        "expected": d.expected,
    })
}

// ---------------------------------------------------------------------------
// Exported functions
// ---------------------------------------------------------------------------

/// The version of this binding, as a static NUL-terminated string. Do not free it.
#[unsafe(no_mangle)]
pub extern "C" fn concerto_ffi_version() -> *const c_char {
    concat!(env!("CARGO_PKG_VERSION"), "\0").as_ptr().cast()
}

/// Frees a string returned by any `concerto_*` function. Null is a no-op.
///
/// # Safety
/// `ptr` must be null or a string this library returned, not yet freed.
#[unsafe(no_mangle)]
pub unsafe extern "C" fn concerto_string_free(ptr: *mut c_char) {
    if !ptr.is_null() {
        // SAFETY: `ptr` came from `CString::into_raw`.
        drop(unsafe { CString::from_raw(ptr) });
    }
}

/// Creates a model manager holding only the Concerto system models.
/// Returns null if the engine could not start (it never should).
#[unsafe(no_mangle)]
pub extern "C" fn concerto_model_manager_new() -> *mut ConcertoModelManager {
    catch_unwind(ModelManager::new)
        .ok()
        .and_then(Result::ok)
        .map(|m| Box::into_raw(Box::new(ConcertoModelManager(m))))
        .unwrap_or(std::ptr::null_mut())
}

/// Frees a model manager. Null is a no-op.
///
/// # Safety
/// `ptr` must be null or a live handle from [`concerto_model_manager_new`],
/// with no call on it in flight.
#[unsafe(no_mangle)]
pub unsafe extern "C" fn concerto_model_manager_free(ptr: *mut ConcertoModelManager) {
    if !ptr.is_null() {
        // SAFETY: `ptr` came from `Box::into_raw`.
        drop(unsafe { Box::from_raw(ptr) });
    }
}

/// Loads a batch of models, given as a JSON array of
/// `concerto.metamodel@1.0.0.Model` ASTs, in any import order, then runs
/// semantic validation over the whole manager. On error the batch has no
/// effect. The envelope's value is the batch's namespaces.
///
/// # Safety
/// `ptr` must be a live handle with no other call in flight (this call
/// mutates it); `asts_json` a NUL-terminated string.
#[unsafe(no_mangle)]
pub unsafe extern "C" fn concerto_model_manager_add_models(
    ptr: *mut ConcertoModelManager,
    asts_json: *const c_char,
) -> *mut c_char {
    envelope(|| {
        // SAFETY: forwarded from the caller.
        let mm = unsafe { ptr.as_mut() }.map(|m| &mut m.0).ok_or_else(|| {
            ErrorPayload::boundary(
                "InvalidArgument",
                "ffi-null-handle",
                "the model manager handle is null",
            )
        })?;
        // SAFETY: forwarded from the caller.
        let asts = parse_json(unsafe { arg(asts_json, "asts") }?, "asts")?;
        let asts = match asts {
            json::Value::Array(items) => items,
            single @ json::Value::Object(_) => vec![single],
            _ => {
                return Err(ErrorPayload::boundary(
                    "InvalidArgument",
                    "ffi-invalid-argument",
                    "`asts` must be a model AST or an array of them",
                ));
            }
        };
        mm.add_model_asts(asts.iter().map(|ast| (ast, None)))
            .map_err(|e| ErrorPayload::from(&e))?;
        let namespaces: Vec<&str> = asts
            .iter()
            .filter_map(|ast| ast.get("namespace").and_then(json::Value::as_str))
            .collect();
        Ok(sjson!(namespaces))
    })
}

/// The fully-qualified names of every declaration the manager holds, system
/// types included, in load order.
///
/// # Safety
/// `ptr` must be a live handle.
#[unsafe(no_mangle)]
pub unsafe extern "C" fn concerto_model_manager_declaration_names(
    ptr: *const ConcertoModelManager,
) -> *mut c_char {
    envelope(|| {
        // SAFETY: forwarded from the caller.
        let mm = unsafe { manager(ptr) }?;
        let names: Vec<String> = mm.declarations().map(|(fqn, _)| fqn).collect();
        Ok(sjson!(names))
    })
}

/// Validates a JSON instance and collects every violation
/// (accordproject/concerto#1239). The envelope's value is
/// `{"valid": bool, "diagnostics": [{pointer, code, severity, message, expected}]}`.
/// `options_json` may be null for the default (lenient) options.
///
/// # Safety
/// `ptr` must be a live handle; the strings NUL-terminated or (options) null.
#[unsafe(no_mangle)]
pub unsafe extern "C" fn concerto_model_manager_check_instance(
    ptr: *const ConcertoModelManager,
    instance_json: *const c_char,
    options_json: *const c_char,
) -> *mut c_char {
    envelope(|| {
        // SAFETY: forwarded from the caller.
        let mm = unsafe { manager(ptr) }?;
        // SAFETY: forwarded from the caller.
        let instance = parse_json(unsafe { arg(instance_json, "instance") }?, "instance")?;
        // SAFETY: forwarded from the caller.
        let options = unsafe { options(options_json) }?;
        let report = mm.check_instance(&instance, &options);
        Ok(sjson!({
            "valid": report.is_valid(),
            "diagnostics": report.diagnostics().iter().map(diagnostic_json).collect::<Vec<_>>(),
        }))
    })
}

/// Validates a JSON instance and stops at the first violation, as
/// `Serializer.fromJSON` does. The envelope is `{"ok":true,"value":null}` for
/// a valid instance, and otherwise carries the engine's error, with the
/// accordproject/concerto#1273 `details` of a strict-mode rejection.
///
/// # Safety
/// As [`concerto_model_manager_check_instance`].
#[unsafe(no_mangle)]
pub unsafe extern "C" fn concerto_model_manager_validate_instance(
    ptr: *const ConcertoModelManager,
    instance_json: *const c_char,
    options_json: *const c_char,
) -> *mut c_char {
    envelope(|| {
        // SAFETY: forwarded from the caller.
        let mm = unsafe { manager(ptr) }?;
        // SAFETY: forwarded from the caller.
        let instance = parse_json(unsafe { arg(instance_json, "instance") }?, "instance")?;
        // SAFETY: forwarded from the caller.
        let options = unsafe { options(options_json) }?;
        mm.validate_instance(&instance, &options)
            .map_err(|e| ErrorPayload::from(&e))?;
        Ok(serde_json::Value::Null)
    })
}
