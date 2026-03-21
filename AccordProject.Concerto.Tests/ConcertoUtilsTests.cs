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

namespace AccordProject.Concerto.Tests;

using System;
using AccordProject.Concerto;

[Collection("Sequential")]
public class ConcertoUtilsTests
{
    [Fact]
    public void CanParseUnversionedNamespace()
    {
        var expected = new ConcertoNamespace() {
            Namespace = "org.example",
        }.ToExpectedObject();
        var actual = ConcertoUtils.ParseNamespace("org.example");
        expected.ShouldEqual(actual);
    }

    [Fact]
    public void CanParseVersionedNamespace()
    {
        var expected = new ConcertoNamespace() {
            Namespace = "org.example",
            Version = "1.2.3"
        }.ToExpectedObject();
        var actual = ConcertoUtils.ParseNamespace("org.example@1.2.3");
        expected.ShouldEqual(actual);
    }
         
    [Fact]
    public void CannotParseNamespaceWithEmptyNamespace()
    {
        var ex = Assert.Throws<Exception>(() => ConcertoUtils.ParseNamespace(""));
        Assert.Equal("Invalid namespace \"\"", ex.Message);
    }
         
    [Fact]
    public void CannotParseNamespaceWithEmptyVersion()
    {
        var ex = Assert.Throws<Exception>(() => ConcertoUtils.ParseNamespace("org.example@"));
        Assert.Equal("Invalid namespace \"org.example@\"", ex.Message);
    }

    [Fact]
    public void CanParseUnversionedType()
    {
        var expected = new ConcertoType() {
            Namespace = "org.example",
            Name = "Foo"
        }.ToExpectedObject();
        var actual = ConcertoUtils.ParseType("org.example.Foo");
        expected.ShouldEqual(actual);
    }

    [Fact]
    public void CanParseVersionedType()
    {
        var expected = new ConcertoType() {
            Namespace = "org.example",
            Version = "1.2.3",
            Name = "Foo"
        }.ToExpectedObject();
        var actual = ConcertoUtils.ParseType("org.example@1.2.3.Foo");
        expected.ShouldEqual(actual);
    }
         
    [Fact]
    public void CannotParseTypeWithMissingPeriod()
    {
        var ex = Assert.Throws<Exception>(() => ConcertoUtils.ParseType("org"));
        Assert.Equal("Invalid fully qualified name \"org\"", ex.Message);
    }
         
    [Fact]
    public void CannotParseTypeWithEmptyNamespace()
    {
        var ex = Assert.Throws<Exception>(() => ConcertoUtils.ParseType(".Foo"));
        Assert.Equal("Invalid fully qualified name \".Foo\"", ex.Message);
    }
         
    [Fact]
    public void CannotParseTypeWithEmptyName()
    {
        var ex = Assert.Throws<Exception>(() => ConcertoUtils.ParseType("org.example."));
        Assert.Equal("Invalid fully qualified name \"org.example.\"", ex.Message);
    }
         
    [Fact]
    public void CannotParseTypeWithEmptyVersion()
    {
        var ex = Assert.Throws<Exception>(() => ConcertoUtils.ParseType("org.example@.Foo"));
        Assert.Equal("Invalid fully qualified name \"org.example@.Foo\"", ex.Message);
    }

    [Fact]
    public void HasIdentifierTrueForTypeWithIdentifier()
    {
        var employee = new Employee() { EmployeeId = "12345678" };
        Assert.True(ConcertoUtils.HasIdentifier(employee));
    }

    [Fact]
    public void HasIdentifierTrueForTypeWithInheritedIdentifier()
    {
        var manager = new Manager() { EmployeeId = "12345678" };
        Assert.True(ConcertoUtils.HasIdentifier(manager));
    }

    [Fact]
    public void HasIdentifierFalseForTypeWithoutIdentifier()
    {
        var project = new Project() {};
        Assert.False(ConcertoUtils.HasIdentifier(project));
    }

    [Fact]
    public void GetIdentifierReturnsForTypeWithIdentifier()
    {
        var employee = new Employee() { EmployeeId = "12345678" };
        Assert.Equal("12345678", ConcertoUtils.GetIdentifier(employee));
    }

    [Fact]
    public void GetIdentifierReturnsForTypeWithInheritedIdentifier()
    {
        var manager = new Manager() { EmployeeId = "12345678" };
        Assert.Equal("12345678", ConcertoUtils.GetIdentifier(manager));
    }

    [Fact]
    public void GetIdentifierReturnsForTypeWithoutIdentifier()
    {
        var project = new Project() {};
        Assert.Null(ConcertoUtils.GetIdentifier(project));
    }
    
    [Fact]
    public void NormalizeIdentifier_NoOpValues()
    {
        var ids = new (object? id, string expected)[] {
            ("a", "a"),
            ("A", "A"),
            ("ĦĔĽĻŎ", "ĦĔĽĻŎ"),
            ("ǅ", "ǅ"),
            ("ᾩ", "ᾩ"),
            ("〱〱〱〱", "〱〱〱〱"),
            ("जावास्क्रिप्ट", "जावास्क्रिप्ट"),
            ("Ⅶ", "Ⅶ"),
            ("$class", "$class"),
            ("_class", "_class"),
            ("\u03C9", "\u03C9"),
            ("abc", "abc"),
            ("a123", "a123"),
            ("foo$bar", "foo$bar"),
            ("foo_bar", "foo_bar"),
            ("αβγδεζηθ", "αβγδεζηθ"),
            ("foo\u03C9bar", "foo\u03C9bar"),
            ("foo\u03c9bar", "foo\u03c9bar"),
            ("foo‿bar", "foo‿bar"),
            ("पः", "पः"),
            ("CharlesⅢ", "CharlesⅢ"),
            ("true", "true"),
            ("false", "false"),
            ("null", "null"),
            ("while", "while"),
            ("for", "for"),
            ("nully", "nully"),
            ("こんにちは世界", "こんにちは世界"),
            ("foo\u200Cbar", "foo_bar"),
            ("foo\u200Dbar", "foo_bar"),
        };
        foreach (var (id, expected) in ids)
        {
            Assert.Equal(expected, ConcertoUtils.NormalizeIdentifier(id, 30));
        }
    }

    [Fact]
    public void NormalizeIdentifier_BadIdentifiers()
    {
        var ids = new (object? id, string expected)[] {
            ("123", "_123"),
            ("1st", "_1st"),
            ("foo bar", "foo_bar"),
            ("foo\u0020bar", "foo_bar"),
            ("\u200Dfoo", "_foo"),
            ("foo-bar", "foo_bar"),
            ("foo\u2010bar", "foo_bar"),
            ("foo\u2212bar", "foo_bar"),
            ("foo|bar", "foo_bar"),
            ("foo@bar", "foo_bar"),
            ("foo#bar", "foo_bar"),
            ("foo/bar", "foo_bar"),
            ("foo>bar", "foo_bar"),
            ("foo.bar", "foo_bar"),
            ("\x3D", "_3d"),
            ("😄", "_1f604"),
        };
        foreach (var (id, expected) in ids)
        {
            Assert.Equal(expected, ConcertoUtils.NormalizeIdentifier(id, 30));
        }
    }

    [Fact]
    public void NormalizeIdentifier_ThrowsForEmptyString()
    {
        Assert.Throws<Exception>(() => ConcertoUtils.NormalizeIdentifier(""));
    }

    [Fact]
    public void NormalizeIdentifier_UnsupportedTypesThrow()
    {
        Assert.Throws<Exception>(() => ConcertoUtils.NormalizeIdentifier(new { a = 1 }));
        Assert.Throws<Exception>(() => ConcertoUtils.NormalizeIdentifier(false));
        Assert.Throws<Exception>(() => ConcertoUtils.NormalizeIdentifier(true));
        Assert.Throws<Exception>(() => ConcertoUtils.NormalizeIdentifier(1));
        Assert.Throws<Exception>(() => ConcertoUtils.NormalizeIdentifier(1.112345678987654));
        Assert.Throws<Exception>(() => ConcertoUtils.NormalizeIdentifier(3.1e2));
    }

    [Fact]
    public void NormalizeIdentifier_Truncation()
    {
        Assert.Equal("a", ConcertoUtils.NormalizeIdentifier("a", 2));
        Assert.Equal("aa", ConcertoUtils.NormalizeIdentifier("aaa", 2));
        Assert.Equal("aaa", ConcertoUtils.NormalizeIdentifier("aaa", 0));
        Assert.Equal("aaa", ConcertoUtils.NormalizeIdentifier("aaa", -1));
        Assert.Equal("$", ConcertoUtils.NormalizeIdentifier("$a", 1));
        Assert.Equal("_1", ConcertoUtils.NormalizeIdentifier("😄", 2));
    }
}