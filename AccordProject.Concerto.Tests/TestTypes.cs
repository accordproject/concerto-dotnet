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
using AccordProject.Concerto;
[AccordProject.Concerto.Type(Namespace = "org.accordproject.concerto.test", Version = "1.2.3", Name = "Person")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public abstract class Person : Participant {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "org.accordproject.concerto.test@1.2.3.Person";
   [Newtonsoft.Json.JsonProperty("email")]
   public string Email { get; set; }
   [Newtonsoft.Json.JsonProperty("firstName")]
   public string FirstName { get; set; }
   [Newtonsoft.Json.JsonProperty("lastName")]
   public string LastName { get; set; }
}
[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum Department {
      MARKETING,
      SALES,
      ENGINEERING,
      OPERATIONS,
}
[AccordProject.Concerto.Type(Namespace = "org.accordproject.concerto.test", Version = "1.2.3", Name = "Employee")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class Employee : Person {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "org.accordproject.concerto.test@1.2.3.Employee";
   [Newtonsoft.Json.JsonProperty("department")]
   public Department? Department { get; set; }
   [Newtonsoft.Json.JsonProperty("manager")]
   public Employee Manager { get; set; }
   [AccordProject.Concerto.Identifier()]
   [Newtonsoft.Json.JsonProperty("employeeId")]
   public string EmployeeId { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "org.accordproject.concerto.test", Version = "1.2.3", Name = "Manager")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class Manager : Employee {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "org.accordproject.concerto.test@1.2.3.Manager";
   [Newtonsoft.Json.JsonProperty("budget")]
   public float? Budget { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "org.accordproject.concerto.test", Version = "1.2.3", Name = "Project")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class Project : Concept {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "org.accordproject.concerto.test@1.2.3.Project";
}
