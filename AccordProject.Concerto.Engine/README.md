# AccordProject.Concerto.Engine (spike)

Concerto instance validation for .NET, running on the
[concerto-rust](https://github.com/accordproject/concerto-rust) engine in-process,
with no Node.js runtime.

This is a proof of concept. It follows the approach
[accordproject/concerto-rust#78](https://github.com/accordproject/concerto-rust/pull/78)
takes for `@accordproject/concerto-core` (a thin binding crate over the Rust
`concerto-core`, with a host-language wrapper on top), using a C ABI and
P/Invoke where the TypeScript side uses WASM and `wasm-bindgen`.

```
 C#  ModelManager / ValidationReport / ConcertoException      AccordProject.Concerto.Engine
       │  LibraryImport (P/Invoke), SafeHandle, UTF-8 JSON
 C   concerto_model_manager_* (cdylib)                         native/concerto-ffi
       │  Rust calls
 Rust concerto_core::ModelManager (native API, no js-compat)   concerto-rust (PR #78)
```

## Usage

```csharp
using AccordProject.Concerto.Engine;

using var models = new ModelManager();

// Model JSON ASTs (concerto.metamodel@1.0.0.Model), in any import order.
// The batch is semantically validated as a whole, and rolled back on error.
models.AddModels(personModelAst, addressModelAst);

// Collect-all validation: a report of diagnostics with JSON Pointers.
ValidationReport report = models.Validate(instanceJson);
foreach (var d in report.Diagnostics)
{
    Console.WriteLine($"{d.Pointer} [{d.Code}] {d.Message}");
}

// First-error validation, as concerto-core's Serializer.fromJSON throws.
// Validation is always strict (Concerto v5): unknown keys and required nulls
// are rejected, with structured details.
try
{
    models.ValidateOrThrow(instanceJson);
}
catch (ConcertoException e) when (e.Kind == "Validation")
{
    Console.WriteLine($"{e.Code}: {e.Message}");
    foreach (var detail in e.Details) // accordproject/concerto#1273 details
    {
        Console.WriteLine($"  {detail.Path} {detail.Code}");
    }
}
```

`samples/ValidateInstance` is a runnable version of this:

```sh
dotnet run --project samples/ValidateInstance
```

## Building

You need the .NET 8 SDK and a Rust toolchain (1.88 or later). `dotnet build`
runs `cargo build --release` for `native/concerto-ffi` and copies the native
library (`libconcerto_ffi.so`, `libconcerto_ffi.dylib` or `concerto_ffi.dll`)
next to the assembly.

```sh
dotnet test AccordProject.Concerto.Engine.sln
```

To use a prebuilt library instead, pass `-p:ConcertoSkipCargo=true
-p:ConcertoNativeDir=<dir>/`.

## The native boundary

`native/concerto-ffi/src/lib.rs` is the whole contract:

| Export | Purpose |
|---|---|
| `concerto_model_manager_new` / `_free` | Opaque handle, owned by a `SafeHandle` |
| `concerto_model_manager_add_models` | `ModelManager::add_model_asts` (load and validate a batch) |
| `concerto_model_manager_declaration_names` | Introspection smoke test |
| `concerto_model_manager_check_instance` | `ModelManager::check_instance` (collect-all, #1239) |
| `concerto_model_manager_validate_instance` | `ModelManager::validate_instance` (first error, #1273 details) |
| `concerto_string_free`, `concerto_ffi_version` | Memory and version |

Arguments are UTF-8 JSON strings. Results are a JSON envelope,
`{"ok":true,"value":...}` or `{"ok":false,"error":{kind,code,message,fileName,details}}`,
which the wrapper turns into a value or a `ConcertoException`. Panics are caught
at the boundary and reported as `Internal` errors.

## Not in the spike

- CTO parsing: models are loaded from their JSON AST, as in concerto-rust's
  native API.
- Packaging: a NuGet package would carry prebuilt libraries under
  `runtimes/<rid>/native/` for each platform, built in CI.
- Introspection, typed instances, and the decorator command sets, which the
  Rust engine supports but this binding does not expose yet.
- Replacing `AccordProject.Concerto.Validate` (Node.js via Jering) or the
  hand-written `AccordProject.Concerto` introspection with this engine.
