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

using System.Reflection;
using AccordProject.Concerto;

public class IntrospectionTests
{
    private readonly ModelManager modelManager = new(new[] { typeof(Employee).Assembly, typeof(Concept).Assembly });

    [Fact]
    public void ReturnsModelFilesByNamespace()
    {
        var modelFile = modelManager.GetModelFile("org.accordproject.concerto.test");

        Assert.NotNull(modelFile);
        Assert.Equal("1.2.3", modelFile!.GetVersion());
        Assert.Contains(modelFile.GetClassDeclarations(), declaration => declaration.Name == "Employee");
        Assert.Contains(modelFile.GetEnumDeclarations(), declaration => declaration.Name == "Department");
    }

    [Fact]
    public void ReturnsClassDeclarationsFromIntrospector()
    {
        var declaration = modelManager.GetIntrospector().GetClassDeclaration("org.accordproject.concerto.test@1.2.3.Employee");

        Assert.Equal("Employee", declaration.Name);
        Assert.Equal("org.accordproject.concerto.test", declaration.Namespace);
        Assert.Equal("org.accordproject.concerto.test@1.2.3.Person", declaration.GetSuperType());
        Assert.True(declaration.IsParticipant());
        Assert.True(declaration.IsIdentified());
        Assert.True(declaration.IsExplicitlyIdentified());
        Assert.Equal("employeeId", declaration.GetIdentifierFieldName());
        Assert.Contains(declaration.GetDecorators(), decorator => decorator.Name == "indexed");
    }

    [Fact]
    public void ReturnsInheritedAndOwnProperties()
    {
        var declaration = modelManager.GetIntrospector().GetClassDeclaration("org.accordproject.concerto.test@1.2.3.Employee");

        var ownProperties = declaration.GetOwnProperties().Select(property => property.Name).ToArray();
        var properties = declaration.GetProperties();

        Assert.Contains("department", ownProperties);
        Assert.Contains("manager", ownProperties);
        Assert.Contains("employeeId", ownProperties);
        Assert.Contains(properties, property => property.Name == "firstName");
        Assert.Contains(properties, property => property.Name == "$identifier");
    }

    [Fact]
    public void ReturnsPropertyMetadata()
    {
        var declaration = modelManager.GetIntrospector().GetClassDeclaration("org.accordproject.concerto.test@1.2.3.Employee");
        var department = declaration.GetProperty("department");
        var manager = declaration.GetProperty("manager");
        var employeeId = declaration.GetProperty("employeeId");

        Assert.NotNull(department);
        Assert.True(department!.IsOptional());
        Assert.True(department.IsTypeEnum());
        Assert.Equal("Department", department.TypeName);
        Assert.Equal("org.accordproject.concerto.test@1.2.3.Department", department.GetFullyQualifiedTypeName());

        Assert.NotNull(manager);
        Assert.False(manager!.IsPrimitive());
        Assert.True(manager.IsRelationship());
        Assert.Equal("Employee", manager.TypeName);
        Assert.Equal("org.accordproject.concerto.test@1.2.3.Employee", manager.GetFullyQualifiedTypeName());

        Assert.NotNull(employeeId);
        Assert.Equal("String", employeeId!.TypeName);
        Assert.Contains(employeeId.GetDecorators(), decorator => decorator.Name == "searchable");
    }

    [Fact]
    public void ReturnsAssignableDeclarations()
    {
        var declaration = modelManager.GetIntrospector().GetClassDeclaration("org.accordproject.concerto.test@1.2.3.Employee");
        var assignable = declaration.GetAssignableClassDeclarations().Select(item => item.Name).ToArray();
        var directSubclasses = declaration.GetDirectSubclasses().Select(item => item.Name).ToArray();

        Assert.Equal(new[] { "Employee", "Manager" }, assignable);
        Assert.Equal(new[] { "Manager" }, directSubclasses);
    }

    [Fact]
    public void CanRegisterTypesExplicitly()
    {
        var localModelManager = new ModelManager(Array.Empty<Assembly>());

        localModelManager.RegisterType(typeof(Employee));
        localModelManager.RegisterType(typeof(Department));

        Assert.NotNull(localModelManager.GetType("org.accordproject.concerto.test@1.2.3.Employee"));
        Assert.NotNull(localModelManager.GetEnum("org.accordproject.concerto.test@1.2.3.Department"));
    }

    [Fact]
    public void ThrowsForUnknownClassDeclaration()
    {
        Assert.Throws<KeyNotFoundException>(() =>
            modelManager.GetIntrospector().GetClassDeclaration("org.accordproject.concerto.test@1.2.3.DoesNotExist"));
    }

    [Fact]
    public void ReturnsOwningModelFileFromDeclaration()
    {
        var declaration = modelManager.GetIntrospector().GetClassDeclaration("org.accordproject.concerto.test@1.2.3.Employee");
        var modelFile = declaration.GetModelFile();

        Assert.Equal("org.accordproject.concerto.test", modelFile.GetNamespace());
        Assert.Equal("1.2.3", modelFile.GetVersion());
        Assert.Contains(modelFile.GetClassDeclarations(), item => item.FullyQualifiedName == declaration.FullyQualifiedName);
    }

    [Fact]
    public void ReturnsDecoratorsByName()
    {
        var declaration = modelManager.GetIntrospector().GetClassDeclaration("org.accordproject.concerto.test@1.2.3.Employee");
        var classDecorator = declaration.GetDecorator("indexed");
        var missingDecorator = declaration.GetDecorator("missing");
        var propertyDecorator = declaration.GetProperty("employeeId")!.GetDecorator("searchable");

        Assert.NotNull(classDecorator);
        Assert.Equal("indexed", classDecorator!.Name);
        Assert.Single(classDecorator.Arguments);
        Assert.Equal("email", classDecorator.Arguments[0]);
        Assert.Null(missingDecorator);
        Assert.NotNull(propertyDecorator);
        Assert.Equal("searchable", propertyDecorator!.Name);
    }

    [Fact]
    public void ReturnsCompleteSuperTypeChain()
    {
        var manager = modelManager.GetIntrospector().GetClassDeclaration("org.accordproject.concerto.test@1.2.3.Manager");
        var superTypes = manager.GetAllSuperTypeDeclarations().Select(item => item.FullyQualifiedName).ToArray();

        Assert.Equal(
            new[]
            {
                "org.accordproject.concerto.test@1.2.3.Employee",
                "org.accordproject.concerto.test@1.2.3.Person",
                "concerto@1.0.0.Participant",
                "concerto@1.0.0.Concept",
            },
            superTypes);
    }
}