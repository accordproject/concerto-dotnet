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

using Xunit;
using Concerto.Serialization.Newtonsoft;
using Concerto.Models.org.test;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ConcertoJsonConverterTests.Newtonsoft;

public class Serialize
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
    public void SimpleObject()
    {
        Employee employee = new Employee()
        {
            firstName = "Matt",
            lastName = "Roberts",
            email = "test@example.com",
            _identifier = "test@example.com",
            department = Department.ENGINEERING,
            employeeId = "123"
        };

        string jsonString = JsonConvert.SerializeObject(employee, options);

        Assert.Equal(Regex.Replace(@"{
            ""$class"": ""org.test.Employee"",
            ""department"": ""ENGINEERING"",
            ""employeeId"": ""123"",
            ""email"": ""test@example.com"",
            ""firstName"": ""Matt"",
            ""lastName"": ""Roberts"",
            ""$identifier"": ""test@example.com""
        }", @"\s+", ""), jsonString);
    }

    [Fact]
    public void PolymorphicObject()
    {
        Employee employee = new Employee()
        {
            firstName = "Matt",
            lastName = "Roberts",
            email = "test@example.com",
            _identifier = "test@example.com",
            department = Department.ENGINEERING,
            employeeId = "123",
            manager = new Employee(){
                email = "test@example.com",
                employeeId = "456",
                firstName = "Martin",
                lastName = "Halford",
            }
        };

        string jsonString = JsonConvert.SerializeObject(employee, options);
        Assert.Equal(Regex.Replace(@"{
            ""$class"": ""org.test.Employee"",
            ""department"": ""ENGINEERING"",
            ""manager"": {
                ""$class"": ""org.test.Employee"",
                ""employeeId"": ""456"",
                ""email"": ""test@example.com"",
                ""firstName"": ""Martin"",
                ""lastName"": ""Halford""
            },
            ""employeeId"": ""123"",
            ""email"": ""test@example.com"",
            ""firstName"": ""Matt"",
            ""lastName"": ""Roberts"",
            ""$identifier"": ""test@example.com""
        }", @"\s+", ""), jsonString);
    }
}