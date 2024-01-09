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
using AccordProject.Concerto.Metamodel;

[Collection("Sequential")]
public class ConcertoTypeDictionaryTests
{
    [Fact]
    public void ReturnsBaseTypeFromString()
    {
        var dictionary = ConcertoTypeDictionary.Instance;
        var type = dictionary.ResolveType("concerto@1.0.0.Concept");
        Assert.Equal(type, typeof(Concept));
    }

    [Fact]
    public void ReturnsBaseTypeFromStruct()
    {
        var dictionary = ConcertoTypeDictionary.Instance;
        var type = dictionary.ResolveType(new ConcertoType() {
            Namespace = "concerto",
            Version = "1.0.0",
            Name = "Concept"
        });
        Assert.Equal(type, typeof(Concept));
    }

    [Fact]
    public void ReturnsMetamodelTypeFromString()
    {
        var dictionary = ConcertoTypeDictionary.Instance;
        var type = dictionary.ResolveType("concerto.metamodel@1.0.0.ConceptDeclaration");
        Assert.Equal(type, typeof(ConceptDeclaration));
    }

    [Fact]
    public void ReturnsMetamodelTypeFromStruct()
    {
        var dictionary = ConcertoTypeDictionary.Instance;
        var type = dictionary.ResolveType(new ConcertoType() {
            Namespace = "concerto.metamodel",
            Version = "1.0.0",
            Name = "ConceptDeclaration"
        });
        Assert.Equal(type, typeof(ConceptDeclaration));
    }

    [Fact]
    public void ReturnsNullForMissingTypeFromString()
    {
        var dictionary = ConcertoTypeDictionary.Instance;
        var type = dictionary.ResolveType("nosuchmodel@1.0.0.NoSuchType");
        Assert.Null(type);
    }

    [Fact]
    public void ReturnsNullForMissingTypeFromStruct()
    {
        var dictionary = ConcertoTypeDictionary.Instance;
        var type = dictionary.ResolveType(new ConcertoType() {
            Namespace = "nosuchmodel",
            Version = "1.0.0",
            Name = "NoSuchType"
        });
        Assert.Null(type);
    }
}