using System;
using System.Text.Json.Serialization;
using Concerto.Serialization;
using NewtonsoftJson = Newtonsoft.Json;
using NewtonsoftConcerto = Concerto.Serialization.Newtonsoft;
namespace Concerto.Models.org.test {
   using Concerto.Models.concerto;
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public abstract class Person : Participant {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "org.test.Person";
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

   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class Employee : Person {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "org.test.Employee";
      public Department? department { get; set; }
      public Employee manager { get; set; }
      public string employeeId { get; set; }
   }
   public class Manager : Employee {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "org.test.Manager";
      public float budget { get; set; }
   }
}
