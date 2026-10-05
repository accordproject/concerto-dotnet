/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Text.Json;
using AccordProject.Concerto.Engine.Native;

namespace AccordProject.Concerto.Engine;

/// <summary>
/// A set of Concerto models, loaded into the native concerto-rust engine,
/// against which JSON instances are validated.
/// </summary>
/// <remarks>
/// Validation calls may run concurrently; <see cref="AddModels(IEnumerable{string})"/>
/// takes an exclusive lock, since it mutates the native manager.
/// </remarks>
public sealed class ModelManager : IDisposable
{
    private readonly ModelManagerSafeHandle handle;
    private readonly ReaderWriterLockSlim gate = new();
    private bool disposed;

    /// <summary>Creates a manager holding only the Concerto system models.</summary>
    public ModelManager()
    {
        handle = NativeMethods.ModelManagerNew();
        if (handle.IsInvalid)
        {
            throw new InvalidOperationException("The Concerto engine failed to create a model manager.");
        }
    }

    /// <summary>The version of the native binding.</summary>
    public static string EngineVersion =>
        System.Runtime.InteropServices.Marshal.PtrToStringUTF8(NativeMethods.Version()) ?? "unknown";

    /// <summary>
    /// Loads models from their JSON ASTs (<c>concerto.metamodel@1.0.0.Model</c>),
    /// in any import order, then validates the whole model set. If any model
    /// is invalid, none of the batch is kept.
    /// </summary>
    /// <returns>The namespaces of the models added.</returns>
    /// <exception cref="ConcertoException">A model is malformed or semantically invalid.</exception>
    public IReadOnlyList<string> AddModels(IEnumerable<string> modelAstsJson)
    {
        var array = "[" + string.Join(",", modelAstsJson) + "]";
        return Write(() => Envelope.Unwrap<string[]>(NativeMethods.ModelManagerAddModels(handle, array)))
            ?? Array.Empty<string>();
    }

    /// <inheritdoc cref="AddModels(IEnumerable{string})"/>
    public IReadOnlyList<string> AddModels(params string[] modelAstsJson) => AddModels((IEnumerable<string>)modelAstsJson);

    /// <summary>The fully-qualified names of every declaration loaded, system types included.</summary>
    public IReadOnlyList<string> DeclarationNames =>
        Read(() => Envelope.Unwrap<string[]>(NativeMethods.ModelManagerDeclarationNames(handle))) ?? Array.Empty<string>();

    /// <summary>
    /// Validates a JSON instance against the loaded models and reports every violation.
    /// </summary>
    /// <exception cref="ConcertoException">The instance is not JSON, or the options are malformed.</exception>
    public ValidationReport Validate(string instanceJson, ValidationOptions? options = null)
    {
        var optionsJson = Serialize(options);
        return Read(() => Envelope.Unwrap<ValidationReport>(
            NativeMethods.ModelManagerCheckInstance(handle, instanceJson, optionsJson)))!;
    }

    /// <summary>
    /// Validates a JSON instance against the loaded models and throws on the
    /// first violation, as concerto-core's <c>Serializer.fromJSON</c> does.
    /// </summary>
    /// <exception cref="ConcertoException">The instance is not valid; <see cref="ConcertoException.Kind"/> is <c>Validation</c>.</exception>
    public void ValidateOrThrow(string instanceJson, ValidationOptions? options = null)
    {
        var optionsJson = Serialize(options);
        Read(() => Envelope.Unwrap<object>(
            NativeMethods.ModelManagerValidateInstance(handle, instanceJson, optionsJson)));
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (disposed)
        {
            return;
        }
        disposed = true;
        gate.EnterWriteLock();
        try
        {
            handle.Dispose();
        }
        finally
        {
            gate.ExitWriteLock();
        }
        gate.Dispose();
    }

    private static string? Serialize(ValidationOptions? options) =>
        options is null ? null : JsonSerializer.Serialize(options, Envelope.JsonOptions);

    private T Read<T>(Func<T> body)
    {
        gate.EnterReadLock();
        try
        {
            ObjectDisposedException.ThrowIf(handle.IsClosed, this);
            return body();
        }
        finally
        {
            gate.ExitReadLock();
        }
    }

    private T Write<T>(Func<T> body)
    {
        gate.EnterWriteLock();
        try
        {
            ObjectDisposedException.ThrowIf(handle.IsClosed, this);
            return body();
        }
        finally
        {
            gate.ExitWriteLock();
        }
    }
}
