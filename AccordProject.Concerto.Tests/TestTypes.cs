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

namespace AccordProject.Concerto.Tests {
   using AccordProject.Concerto;
   [AccordProject.Concerto.Type(Namespace = "org.accordproject.concerto.test", Version = "1.2.3", Name = "Person")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public abstract class Person : Participant {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "org.accordproject.concerto.test@1.2.3.Person";
      public string email { get; set; }
      public string firstName { get; set; }
      public string lastName { get; set; }
   }
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
		public override string _class { get; } = "org.accordproject.concerto.test@1.2.3.Employee";
      public Department? department { get; set; }
      public Employee manager { get; set; }
      public string employeeId { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "org.accordproject.concerto.test", Version = "1.2.3", Name = "Manager")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public class Manager : Employee {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "org.accordproject.concerto.test@1.2.3.Manager";
      public float budget { get; set; }
   }
}
