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
using Concerto.Serialization.Newtonsoft;
using Concerto.Models.org.test;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ConcertoJsonConverterTests.Newtonsoft;

[Collection("Sequential")]
public class Deserialize
{
    JsonSerializerSettings options = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        Converters =
        {
            new StringEnumConverter(),
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

        var employee = JsonConvert.DeserializeObject<Employee>(jsonString, options);

        Assert.NotNull(employee);
        Assert.Equal("Matt", employee!.firstName);
        Assert.Equal("Roberts", employee.lastName);
        Assert.Equal("test@example.com", employee.email);
        Assert.Equal("test@example.com", employee._identifier);
        Assert.Equal(Department.ENGINEERING, employee.department);
        Assert.Equal("123", employee.employeeId);
    }


    // This test does not work because we use the default Newtonsoft JSON deserializer recursively. 
    // We would have to copy the System.Text.Json approach to explicitly visit the properties to have this
    // test pass. 
    // [Fact]
    // public void SimpleObject_DeserializeToSuperType_Succeeds()
    // {
    //     string jsonString = @"{
    //         ""$class"": ""org.test.Manager"",
    //         ""department"": ""ENGINEERING"",
    //         ""employeeId"": ""123"",
    //         ""email"": ""test@example.com"",
    //         ""firstName"": ""Matt"",
    //         ""lastName"": ""Roberts"",
    //         ""budget"": 1.00,
    //         ""$identifier"": ""test@example.com""
    //     }";

    //     Manager employee = (Manager) JsonConvert.DeserializeObject<Employee>(jsonString, options);


    //     employee.ToExpectedObject().ShouldEqual(new Manager()
    //     {
    //         firstName = "Matt",
    //         lastName = "Roberts",
    //         email = "test@example.com",
    //         _identifier = "test@example.com",
    //         department = Department.ENGINEERING,
    //         employeeId = "123",
    //         budget = 1
    //     });

    //     // Should not throw
    //     Manager manager = (Manager) employee;
    // }

    [Fact]
    public void MissingTypeCannotBeDeserialized()
    {
        string jsonString = @"{
            ""$class"": ""org.test.Foo""
        }";

        var ex = Assert.Throws<JsonException>(() => JsonConvert.DeserializeObject<Employee>(jsonString, options));

        Assert.Equal("Type definition `org.test.Foo` not found.", ex.Message);
    }

    // Skipping because of weird bug where this exception is "randomly" swallowed! :-O
    // [Fact]
    // public void ScalarTypeCannotBeDeserialized()
    // {
    //     string jsonString = "true";

    //     var ex = Assert.Throws<JsonException>(() => JsonConvert.DeserializeObject<Employee>(jsonString, options));

    //     Assert.Equal("Only JSON Objects can be deserialized with ConcertoConverter.", ex.Message);
    // }

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

        var ex = Assert.Throws<JsonException>(() => JsonConvert.DeserializeObject<Employee>(jsonString, options));

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

        var employee = JsonConvert.DeserializeObject<Employee>(jsonStringWithManager, options);

        Assert.NotNull(employee);
        Assert.Equal("Matt", employee!.firstName);
        Assert.Equal("Roberts", employee.lastName);
        Assert.Equal("test@example.com", employee.email);
        Assert.Equal("test@example.com", employee._identifier);
        Assert.Equal(Department.ENGINEERING, employee.department);
        Assert.Equal("123", employee.employeeId);
        Assert.NotNull(employee.manager);
        Assert.Equal("test@example.com", employee.manager!.email);
        Assert.Equal("test@example.com", employee.manager._identifier);
        Assert.Equal("456", employee.manager.employeeId);
        Assert.Equal("Martin", employee.manager.firstName);
        Assert.Equal("Halford", employee.manager.lastName);
    }

    // Not yet implemented
    // [Fact]
    // public void NotSubType_Fails()
    // {
    //     string jsonString = @"
    //     {
    //        // TODO
    //     }";

    //      var ex = Assert.Throws<JsonException>(() => JsonConvert.DeserializeObject<Employee>(jsonString, options));

    //     Assert.Equal("JSON Object is missing `$class` property.", ex.Message);

    // }
}