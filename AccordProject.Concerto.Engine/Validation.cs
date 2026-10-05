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

namespace AccordProject.Concerto.Engine;

/// <summary>How an instance is validated. Every option is off by default.</summary>
public sealed record ValidationOptions
{
    /// <summary>The accordproject/concerto#1273 strict preset: unknown keys and required nulls are rejected.</summary>
    public static ValidationOptions Strict { get; } = new() { RejectUnknownKeys = true, RejectRequiredNull = true };

    /// <summary>A key the declaration does not declare is an error.</summary>
    public bool RejectUnknownKeys { get; init; }

    /// <summary>A required property explicitly set to <c>null</c> is an error.</summary>
    public bool RejectRequiredNull { get; init; }

    /// <summary>A relationship property may hold a resource instead of a relationship URI.</summary>
    public bool PermitResourcesForRelationships { get; init; }

    /// <summary>A relationship property may hold a resource, converted to a relationship.</summary>
    public bool ConvertResourcesToRelationships { get; init; }
}

/// <summary>How serious a <see cref="Diagnostic"/> is.</summary>
public enum Severity
{
    Error,
    Warning,
}

/// <summary>
/// One violation found while validating an instance.
/// </summary>
/// <param name="Pointer">An RFC 6901 JSON Pointer to the offending value (<c>""</c> for the root, <c>"/name"</c>, <c>"/tags/0"</c>).</param>
/// <param name="Code">A stable code, such as <c>TYPE_VIOLATION</c> or <c>MISSING_REQUIRED_PROPERTY</c>.</param>
/// <param name="Severity">How serious it is.</param>
/// <param name="Message">A human-readable description, worded as concerto-core words it.</param>
/// <param name="Expected">The declared type at <paramref name="Pointer"/>, when the engine reports one.</param>
public sealed record Diagnostic(string Pointer, string Code, Severity Severity, string Message, string? Expected);

/// <summary>Every <see cref="Diagnostic"/> found in an instance (accordproject/concerto#1239).</summary>
public sealed record ValidationReport(bool Valid, IReadOnlyList<Diagnostic> Diagnostics)
{
    /// <summary>True when no error-severity diagnostic was found.</summary>
    public bool IsValid => Valid;
}
