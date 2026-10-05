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
using System.Text.Json.Serialization;

namespace AccordProject.Concerto.Engine.Native;

/// <summary>Decodes the <c>{"ok":...}</c> envelope every native call returns.</summary>
internal static class Envelope
{
    internal static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() },
    };

    private sealed record Raw(bool Ok, JsonElement? Value, ErrorPayload? Error);

    internal sealed record ErrorPayload(
        string Kind,
        string Code,
        string Message,
        string? FileName,
        IReadOnlyList<ValidationDetail>? Details);

    /// <summary>Returns the envelope's value, or throws its error as a <see cref="ConcertoException"/>.</summary>
    internal static T? Unwrap<T>(IntPtr ptr)
    {
        var raw = JsonSerializer.Deserialize<Raw>(NativeMethods.TakeString(ptr), JsonOptions)
            ?? throw new InvalidOperationException("The Concerto engine returned an empty envelope.");
        if (!raw.Ok)
        {
            var e = raw.Error ?? new ErrorPayload("Internal", "ffi-missing-error", "Unknown engine error.", null, null);
            throw new ConcertoException(e.Kind, e.Code, e.Message, e.FileName, e.Details ?? Array.Empty<ValidationDetail>());
        }
        return raw.Value is { } value ? value.Deserialize<T>(JsonOptions) : default;
    }
}
