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

using System;
using Xunit;
using ExpectedObjects;
using Concerto.Serialization;
using Concerto.Models.org.test;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ConcertoJsonConverterTests;

public class Deserialize
{
    JsonSerializerOptions options = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters =
        {
            new JsonStringEnumConverter(),
            new ConcertoConverter(),
        }
    };

    [Fact]
    public void SimpleObject_Succeeds()
    {
        string jsonString = @"{
            ""$class"": ""org.test.Employee"",
            ""department"": ""ENGINEERING"",
            ""employeeId"": ""123"",
            ""email"": ""test@example.com"",
            ""firstName"": ""Matt"",
            ""lastName"": ""Roberts"",
            ""$identifier"": ""test@example.com""
        }";

        var employee = JsonSerializer.Deserialize<Employee>(jsonString, options).ToExpectedObject();

        employee.ShouldEqual(new Employee()
        {
            firstName = "Matt",
            lastName = "Roberts",
            email = "test@example.com",
            _identifier = "test@example.com",
            department = Department.ENGINEERING,
            employeeId = "123"
        });
    }

    [Fact]
    public void SimpleObject_DeserializeToSuperType_Succeeds()
    {
        string jsonString = @"{
            ""$class"": ""org.test.Manager"",
            ""department"": ""ENGINEERING"",
            ""employeeId"": ""123"",
            ""email"": ""test@example.com"",
            ""firstName"": ""Matt"",
            ""lastName"": ""Roberts"",
            ""budget"": 1.00,
            ""$identifier"": ""test@example.com""
        }";

        Employee employee = JsonSerializer.Deserialize<Employee>(jsonString, options);
        
        Assert.Equal(employee.GetType(), typeof(Manager));

        employee.ToExpectedObject().ShouldEqual(new Manager()
        {
            firstName = "Matt",
            lastName = "Roberts",
            email = "test@example.com",
            _identifier = "test@example.com",
            department = Department.ENGINEERING,
            employeeId = "123",
            budget = 1
        });

        // Should not throw
        Manager manager = (Manager) employee;
    }

    [Fact]
    public void MissingTypeCannotBeDeserialized()
    {
        string jsonString = @"{
            ""$class"": ""org.test.Foo""
        }";

        var ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Employee>(jsonString, options));
 
        Assert.Equal("Type definition `org.test.Foo` not found.", ex.Message);
    }

    [Fact]
    public void ScalarTypeCannotBeDeserialized()
    {
        string jsonString = "true";

        var ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Employee>(jsonString, options));
 
        Assert.Equal("Only JSON Objects can be deserialized with ConcertoConverter.", ex.Message);
    }

    [Fact]
    public void MissingClassProperty()
    {
        string jsonString = @"{
            ""department"": ""ENGINEERING"",
            ""employeeId"": ""123"",
            ""email"": ""test@example.com"",
            ""firstName"": ""Matt"",
            ""lastName"": ""Roberts"",
            ""$identifier"": ""test@example.com""
        }";

        var ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Employee>(jsonString, options));
 
        Assert.Equal("JSON Object is missing `$class` property.", ex.Message);
    }

    [Fact]
    public void PolymorphicObject_Succeeds()
    {
        string jsonStringWithManager = @"
        {
            ""$class"": ""org.test.Employee"",
            ""department"": ""ENGINEERING"",
            ""manager"": {
                ""$class"": ""org.test.Employee"",
                ""email"": ""test@example.com"",
                ""firstName"": ""Martin"",
                ""lastName"": ""Halford"",
                ""$identifier"": ""test@example.com"",
                ""employeeId"": ""456""
            },
            ""employeeId"": ""123"",
            ""email"": ""test@example.com"",
            ""firstName"": ""Matt"",
            ""lastName"": ""Roberts"",
            ""$identifier"": ""test@example.com""
        }";

        var employee = JsonSerializer.Deserialize<Employee>(jsonStringWithManager, options).ToExpectedObject();
        
        employee.ShouldEqual(new Employee()
        {
            firstName = "Matt",
            lastName = "Roberts",
            email = "test@example.com",
            _identifier = "test@example.com",
            department = Department.ENGINEERING,
            employeeId = "123",
            manager = new Employee(){
                email = "test@example.com",
                _identifier = "test@example.com",
                employeeId = "456",
                firstName = "Martin",
                lastName = "Halford",
            }
        });
    }

    // Not yet implemented
    // [Fact]
    // public void NotSubType_Fails()
    // {
    //     string jsonString = @"
    //     {
    //        // TODO
    //     }";

    //      var ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Employee>(jsonString, options));
 
    //     Assert.Equal("JSON Object is missing `$class` property.", ex.Message);

    // }
}