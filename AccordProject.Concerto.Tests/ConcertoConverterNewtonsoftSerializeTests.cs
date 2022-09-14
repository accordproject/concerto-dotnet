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

using System.Text.RegularExpressions;
using AccordProject.Concerto;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class ConcertoConverterNewtonsoftSerializeTests
{
    [Fact]
    public void SimpleObject()
    {
        Employee employee = new Employee()
        {
            FirstName = "Matt",
            LastName = "Roberts",
            Email = "test@example.com",
            _Identifier = "test@example.com",
            Department = Department.ENGINEERING,
            EmployeeId = "123"
        };

        string jsonString = JsonConvert.SerializeObject(employee);

        Assert.Equal(Regex.Replace(@"{
            ""$class"": ""org.accordproject.concerto.test@1.2.3.Employee"",
            ""department"": ""ENGINEERING"",
            ""manager"": null,
            ""employeeId"": ""123"",
            ""email"": ""test@example.com"",
            ""firstName"": ""Matt"",
            ""lastName"": ""Roberts"",
            ""$identifier"": ""test@example.com""
        }", @"\s+", ""), jsonString);
    }

    [Fact]
    public void ComplexObject()
    {
        Employee employee = new Employee()
        {
            FirstName = "Matt",
            LastName = "Roberts",
            Email = "test@example.com",
            _Identifier = "test@example.com",
            Department = Department.ENGINEERING,
            EmployeeId = "123",
            Manager = new Employee(){
                Email = "test@example.com",
                EmployeeId = "456",
                FirstName = "Martin",
                LastName = "Halford",
            }
        };

        string jsonString = JsonConvert.SerializeObject(employee);
        Assert.Equal(Regex.Replace(@"{
            ""$class"": ""org.accordproject.concerto.test@1.2.3.Employee"",
            ""department"": ""ENGINEERING"",
            ""manager"": {
                ""$class"": ""org.accordproject.concerto.test@1.2.3.Employee"",
                ""department"": null,
                ""manager"": null,
                ""employeeId"": ""456"",
                ""email"": ""test@example.com"",
                ""firstName"": ""Martin"",
                ""lastName"": ""Halford"",
                ""$identifier"": null
            },
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
            FirstName = "Matt",
            LastName = "Roberts",
            Email = "test@example.com",
            _Identifier = "test@example.com",
            Department = Department.ENGINEERING,
            EmployeeId = "123",
            Manager = new Manager(){
                Email = "test@example.com",
                EmployeeId = "456",
                FirstName = "Martin",
                LastName = "Halford",
                Budget = 500000
            }
        };

        string jsonString = JsonConvert.SerializeObject(employee);
        Assert.Equal(Regex.Replace(@"{
            ""$class"": ""org.accordproject.concerto.test@1.2.3.Employee"",
            ""department"": ""ENGINEERING"",
            ""manager"": {
                ""$class"": ""org.accordproject.concerto.test@1.2.3.Manager"",
                ""budget"": 500000.0,
                ""department"": null,
                ""manager"": null,
                ""employeeId"": ""456"",
                ""email"": ""test@example.com"",
                ""firstName"": ""Martin"",
                ""lastName"": ""Halford"",
                ""$identifier"": null
            },
            ""employeeId"": ""123"",
            ""email"": ""test@example.com"",
            ""firstName"": ""Matt"",
            ""lastName"": ""Roberts"",
            ""$identifier"": ""test@example.com""
        }", @"\s+", ""), jsonString);
    }
}