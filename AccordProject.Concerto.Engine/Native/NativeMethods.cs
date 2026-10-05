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

using System.Runtime.InteropServices;

namespace AccordProject.Concerto.Engine.Native;

/// <summary>
/// The C ABI exported by native/concerto-ffi (see its src/lib.rs). Every
/// function returning <see cref="IntPtr"/> hands back a UTF-8 JSON envelope
/// that must be released with <see cref="StringFree"/>.
/// </summary>
internal static partial class NativeMethods
{
    internal const string Library = "concerto_ffi";

    [LibraryImport(Library, EntryPoint = "concerto_ffi_version")]
    internal static partial IntPtr Version();

    [LibraryImport(Library, EntryPoint = "concerto_string_free")]
    internal static partial void StringFree(IntPtr ptr);

    [LibraryImport(Library, EntryPoint = "concerto_model_manager_new")]
    internal static partial ModelManagerSafeHandle ModelManagerNew();

    [LibraryImport(Library, EntryPoint = "concerto_model_manager_free")]
    internal static partial void ModelManagerFree(IntPtr handle);

    [LibraryImport(Library, EntryPoint = "concerto_model_manager_add_models", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial IntPtr ModelManagerAddModels(ModelManagerSafeHandle handle, string astsJson);

    [LibraryImport(Library, EntryPoint = "concerto_model_manager_declaration_names")]
    internal static partial IntPtr ModelManagerDeclarationNames(ModelManagerSafeHandle handle);

    [LibraryImport(Library, EntryPoint = "concerto_model_manager_check_instance", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial IntPtr ModelManagerCheckInstance(ModelManagerSafeHandle handle, string instanceJson, string? optionsJson);

    [LibraryImport(Library, EntryPoint = "concerto_model_manager_validate_instance", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial IntPtr ModelManagerValidateInstance(ModelManagerSafeHandle handle, string instanceJson, string? optionsJson);

    /// <summary>Copies a returned envelope into a managed string and frees the native copy.</summary>
    internal static string TakeString(IntPtr ptr)
    {
        if (ptr == IntPtr.Zero)
        {
            throw new InvalidOperationException("The Concerto engine returned no result.");
        }
        try
        {
            return Marshal.PtrToStringUTF8(ptr)!;
        }
        finally
        {
            StringFree(ptr);
        }
    }
}
