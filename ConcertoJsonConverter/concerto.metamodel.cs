using System;
using System.Text.Json.Serialization;
using Concerto.Serialization;
using NewtonsoftJson = Newtonsoft.Json;
using NewtonsoftConcerto = Concerto.Serialization.Newtonsoft;
namespace Concerto.Models.concerto.metamodel {
   using Concerto.Models.concerto;
   public class Position : Concept {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "concerto.metamodel.Position";
      public int line { get; set; }
      public int column { get; set; }
      public int offset { get; set; }
   }
   public class Range : Concept {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "concerto.metamodel.Range";
      public Position start { get; set; }
      public Position end { get; set; }
      public string source { get; set; }
   }
   public class TypeIdentifier : Concept {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "concerto.metamodel.TypeIdentifier";
      public string name { get; set; }
      [JsonPropertyName("namespace")]
		[NewtonsoftJson.JsonProperty("namespace")]
		public string _namespace { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public abstract class DecoratorLiteral : Concept {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "concerto.metamodel.DecoratorLiteral";
      public Range location { get; set; }
   }
   public class DecoratorString : DecoratorLiteral {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "concerto.metamodel.DecoratorString";
      public string value { get; set; }
   }
   public class DecoratorNumber : DecoratorLiteral {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "concerto.metamodel.DecoratorNumber";
      public float value { get; set; }
   }
   public class DecoratorBoolean : DecoratorLiteral {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "concerto.metamodel.DecoratorBoolean";
      public bool value { get; set; }
   }
   public class DecoratorTypeReference : DecoratorLiteral {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "concerto.metamodel.DecoratorTypeReference";
      public TypeIdentifier type { get; set; }
      public bool isArray { get; set; }
   }
   public class Decorator : Concept {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "concerto.metamodel.Decorator";
      public string name { get; set; }
      public DecoratorLiteral[] arguments { get; set; }
      public Range location { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class Identified : Concept {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "concerto.metamodel.Identified";
   }
   public class IdentifiedBy : Identified {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "concerto.metamodel.IdentifiedBy";
      public string name { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public abstract class Declaration : Concept {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "concerto.metamodel.Declaration";
      public string name { get; set; }
      public Decorator[] decorators { get; set; }
      public Range location { get; set; }
   }
   public class EnumDeclaration : Declaration {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "concerto.metamodel.EnumDeclaration";
      public EnumProperty[] properties { get; set; }
   }
   public class EnumProperty : Concept {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "concerto.metamodel.EnumProperty";
      public string name { get; set; }
      public Decorator[] decorators { get; set; }
      public Range location { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class ConceptDeclaration : Declaration {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "concerto.metamodel.ConceptDeclaration";
      public bool isAbstract { get; set; }
      public Identified identified { get; set; }
      public TypeIdentifier superType { get; set; }
      public Property[] properties { get; set; }
   }
   public class AssetDeclaration : ConceptDeclaration {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "concerto.metamodel.AssetDeclaration";
   }
   public class ParticipantDeclaration : ConceptDeclaration {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "concerto.metamodel.ParticipantDeclaration";
   }
   public class TransactionDeclaration : ConceptDeclaration {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "concerto.metamodel.TransactionDeclaration";
   }
   public class EventDeclaration : ConceptDeclaration {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "concerto.metamodel.EventDeclaration";
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public abstract class Property : Concept {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "concerto.metamodel.Property";
      public string name { get; set; }
      public bool isArray { get; set; }
      public bool isOptional { get; set; }
      public Decorator[] decorators { get; set; }
      public Range location { get; set; }
   }
   public class RelationshipProperty : Property {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "concerto.metamodel.RelationshipProperty";
      public TypeIdentifier type { get; set; }
   }
   public class ObjectProperty : Property {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "concerto.metamodel.ObjectProperty";
      public string defaultValue { get; set; }
      public TypeIdentifier type { get; set; }
   }
   public class BooleanProperty : Property {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "concerto.metamodel.BooleanProperty";
      public bool defaultValue { get; set; }
   }
   public class DateTimeProperty : Property {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "concerto.metamodel.DateTimeProperty";
   }
   public class StringProperty : Property {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "concerto.metamodel.StringProperty";
      public string defaultValue { get; set; }
      public StringRegexValidator validator { get; set; }
   }
   public class StringRegexValidator : Concept {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "concerto.metamodel.StringRegexValidator";
      public string pattern { get; set; }
      public string flags { get; set; }
   }
   public class DoubleProperty : Property {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "concerto.metamodel.DoubleProperty";
      public float defaultValue { get; set; }
      public DoubleDomainValidator validator { get; set; }
   }
   public class DoubleDomainValidator : Concept {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "concerto.metamodel.DoubleDomainValidator";
      public float lower { get; set; }
      public float upper { get; set; }
   }
   public class IntegerProperty : Property {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "concerto.metamodel.IntegerProperty";
      public int defaultValue { get; set; }
      public IntegerDomainValidator validator { get; set; }
   }
   public class IntegerDomainValidator : Concept {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "concerto.metamodel.IntegerDomainValidator";
      public int lower { get; set; }
      public int upper { get; set; }
   }
   public class LongProperty : Property {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "concerto.metamodel.LongProperty";
      public long defaultValue { get; set; }
      public LongDomainValidator validator { get; set; }
   }
   public class LongDomainValidator : Concept {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "concerto.metamodel.LongDomainValidator";
      public long lower { get; set; }
      public long upper { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public abstract class Import : Concept {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "concerto.metamodel.Import";
      [JsonPropertyName("namespace")]
		[NewtonsoftJson.JsonProperty("namespace")]
		public string _namespace { get; set; }
      public string uri { get; set; }
   }
   public class ImportAll : Import {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "concerto.metamodel.ImportAll";
   }
   public class ImportType : Import {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "concerto.metamodel.ImportType";
      public string name { get; set; }
   }
   public class Model : Concept {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "concerto.metamodel.Model";
      [JsonPropertyName("namespace")]
		[NewtonsoftJson.JsonProperty("namespace")]
		public string _namespace { get; set; }
      public string sourceUri { get; set; }
      public string concertoVersion { get; set; }
      public Import[] imports { get; set; }
      public Declaration[] declarations { get; set; }
   }
   public class Models : Concept {
      [JsonPropertyName("$class")]
		[NewtonsoftJson.JsonProperty("$class")]
		public override string _class { get;} = "concerto.metamodel.Models";
      public Model[] models { get; set; }
   }
}
