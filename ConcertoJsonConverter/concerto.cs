using System;
using System.Text.Json.Serialization;
using Concerto.Serialization;
using NewtonsoftJson = Newtonsoft.Json;
using NewtonsoftConcerto = Concerto.Serialization.Newtonsoft;

namespace Concerto.Models.concerto {
[NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
public abstract class Concept {
   [JsonPropertyName("$class")]
   [NewtonsoftJson.JsonProperty("$class")]
   public virtual string _class { get; } = "concerto.Concept";
}
public abstract class Asset : Concept {
   [JsonPropertyName("$class")]
   [NewtonsoftJson.JsonProperty("$class")]
   public override string _class { get; } = "concerto.Asset";
   [JsonPropertyName("$identifier")]
   [NewtonsoftJson.JsonProperty("$identifier")]
   public string _identifier { get; set; }
}
[NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
public abstract class Participant : Concept {
   [JsonPropertyName("$class")]
   [NewtonsoftJson.JsonProperty("$class")]
   public override string _class { get; } = "concerto.Participant";
   [JsonPropertyName("$identifier")]
   [NewtonsoftJson.JsonProperty("$identifier")]
   public string _identifier { get; set; }
}
public abstract class Transaction : Concept {
   [JsonPropertyName("$class")]
   [NewtonsoftJson.JsonProperty("$class")]
   public override string _class { get; } = "concerto.Transaction";
   [JsonPropertyName("$timestamp")]
   [NewtonsoftJson.JsonProperty("$timestamp")]
   public System.DateTime _timestamp { get; set; }
}
public abstract class Event : Concept {
   [JsonPropertyName("$class")]
   [NewtonsoftJson.JsonProperty("$class")]
   public override string _class { get; } = "concerto.Event";
   [JsonPropertyName("$timestamp")]
   [NewtonsoftJson.JsonProperty("$timestamp")]
   public System.DateTime _timestamp { get; set; }
}
}
