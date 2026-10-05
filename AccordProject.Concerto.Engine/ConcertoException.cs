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

/// <summary>
/// An error raised by the Concerto engine. <see cref="Kind"/> and
/// <see cref="Code"/> are stable identifiers, safe to match on;
/// the message wording is not.
/// </summary>
public sealed class ConcertoException : Exception
{
    internal ConcertoException(string kind, string code, string message, string? fileName, IReadOnlyList<ValidationDetail> details)
        : base(message)
    {
        Kind = kind;
        Code = code;
        FileName = fileName;
        Details = details;
    }

    /// <summary>
    /// The engine's error kind: <c>IllegalModel</c>, <c>TypeNotFound</c>,
    /// <c>Validation</c>, <c>Metamodel</c>, <c>InvalidArgument</c>,
    /// <c>MalformedInput</c>, or <c>Internal</c> for a failure in the binding itself.
    /// </summary>
    public string Kind { get; }

    /// <summary>The message catalogue key of the error.</summary>
    public string Code { get; }

    /// <summary>The model file the error is about, when there is one.</summary>
    public string? FileName { get; }

    /// <summary>The structured violations of a strict-mode rejection (accordproject/concerto#1273).</summary>
    public IReadOnlyList<ValidationDetail> Details { get; }
}

/// <summary>One structured violation in <see cref="ConcertoException.Details"/>.</summary>
public sealed record ValidationDetail(string Path, string Code, string? Expected, string? Actual);
