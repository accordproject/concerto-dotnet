using System;
using System.Text.Json.Serialization;
using Concerto.Serialization;
using NewtonsoftJson = Newtonsoft.Json;
using NewtonsoftConcerto = Concerto.Serialization.Newtonsoft;
namespace Concerto.Models.concerto.metamodel
{
   using Concerto.Models.concerto;

   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class Position : Concept
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.Position";
      [JsonPropertyName("line")]
      [NewtonsoftJson.JsonProperty("line")]
      public int line { get; set; }
      [JsonPropertyName("column")]
      [NewtonsoftJson.JsonProperty("column")]
      public int column { get; set; }
      [JsonPropertyName("offset")]
      [NewtonsoftJson.JsonProperty("offset")]
      public int offset { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class Range : Concept
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.Range";
      [JsonPropertyName("start")]
      [NewtonsoftJson.JsonProperty("start")]
      public Position start { get; set; }
      [JsonPropertyName("end")]
      [NewtonsoftJson.JsonProperty("end")]
      public Position end { get; set; }
      [JsonPropertyName("source")]
      [NewtonsoftJson.JsonProperty("source")]
      public string? source { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class TypeIdentifier : Concept
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.TypeIdentifier";
      [JsonPropertyName("name")]
      [NewtonsoftJson.JsonProperty("name")]
      public string name { get; set; }
      [JsonPropertyName("namespace")]
      [NewtonsoftJson.JsonProperty("namespace")]
      public string? _namespace  { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public abstract class DecoratorLiteral : Concept
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.DecoratorLiteral";
      [JsonPropertyName("location")]
      [NewtonsoftJson.JsonProperty("location")]
      public Range? location { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class DecoratorString : DecoratorLiteral
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.DecoratorString";
      [JsonPropertyName("value")]
      [NewtonsoftJson.JsonProperty("value")]
      public string value { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class DecoratorNumber : DecoratorLiteral
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.DecoratorNumber";
      [JsonPropertyName("value")]
      [NewtonsoftJson.JsonProperty("value")]
      public float value { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class DecoratorBoolean : DecoratorLiteral
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.DecoratorBoolean";
      [JsonPropertyName("value")]
      [NewtonsoftJson.JsonProperty("value")]
      public bool value { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class DecoratorTypeReference : DecoratorLiteral
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.DecoratorTypeReference";
      [JsonPropertyName("type")]
      [NewtonsoftJson.JsonProperty("type")]
      public TypeIdentifier type { get; set; }
      [JsonPropertyName("isArray")]
      [NewtonsoftJson.JsonProperty("isArray")]
      public bool isArray { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class Decorator : Concept
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.Decorator";
      [JsonPropertyName("name")]
      [NewtonsoftJson.JsonProperty("name")]
      public string name { get; set; }
      [JsonPropertyName("arguments")]
      [NewtonsoftJson.JsonProperty("arguments")]
      public DecoratorLiteral?[] Arguments { get; set; }
      [JsonPropertyName("location")]
      [NewtonsoftJson.JsonProperty("location")]
      public Range? location { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class identified : Concept
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.Identified";
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class identifiedBy : identified
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.IdentifiedBy";
      [JsonPropertyName("name")]
      [NewtonsoftJson.JsonProperty("name")]
      public string name { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public abstract class Declaration : Concept
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.Declaration";
      [System.ComponentModel.DataAnnotations.RegularExpression(@"^(\p{Lu}|\p{Ll}|\p{Lt}|\p{Lm}|\p{Lo}|\p{Nl}|\$|_|\\u[0-9A-Fa-f]{4})(?:\p{Lu}|\p{Ll}|\p{Lt}|\p{Lm}|\p{Lo}|\p{Nl}|\$|_|\\u[0-9A-Fa-f]{4}|\p{Mn}|\p{Mc}|\p{Nd}|\p{Pc}|\u200C|\u200D)*$", ErrorMessage = "Invalid characters")]
      [JsonPropertyName("name")]
      [NewtonsoftJson.JsonProperty("name")]
      public string name { get; set; }
      [JsonPropertyName("decorators")]
      [NewtonsoftJson.JsonProperty("decorators")]
      public Decorator?[] decorators  { get; set; }
      [JsonPropertyName("location")]
      [NewtonsoftJson.JsonProperty("location")]
      public Range? location { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public abstract class MapKeyType : Concept
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.MapKeyType";
      [JsonPropertyName("decorators")]
      [NewtonsoftJson.JsonProperty("decorators")]
      public Decorator?[] decorators  { get; set; }
      [JsonPropertyName("location")]
      [NewtonsoftJson.JsonProperty("location")]
      public Range? location { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public abstract class MapvalueType : Concept
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.MapvalueType";
      [JsonPropertyName("decorators")]
      [NewtonsoftJson.JsonProperty("decorators")]
      public Decorator?[] decorators  { get; set; }
      [JsonPropertyName("location")]
      [NewtonsoftJson.JsonProperty("location")]
      public Range? location { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class MapDeclaration : Declaration
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.MapDeclaration";
      [JsonPropertyName("key")]
      [NewtonsoftJson.JsonProperty("key")]
      public MapKeyType  key  { get; set; }
      [JsonPropertyName("value")]
      [NewtonsoftJson.JsonProperty("value")]
      public MapvalueType value { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class StringMapKeyType : MapKeyType
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.StringMapKeyType";
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class DateTimeMapKeyType : MapKeyType
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.DateTimeMapKeyType";
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class ObjectMapKeyType : MapKeyType
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.ObjectMapKeyType";
      [JsonPropertyName("type")]
      [NewtonsoftJson.JsonProperty("type")]
      public TypeIdentifier type { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class BooleanMapvalueType : MapvalueType
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.BooleanMapvalueType";
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class DateTimeMapvalueType : MapvalueType
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.DateTimeMapvalueType";
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class StringMapvalueType : MapvalueType
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.StringMapvalueType";
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class IntegerMapvalueType : MapvalueType
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.IntegerMapvalueType";
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class LongMapvalueType : MapvalueType
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.LongMapvalueType";
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class DoubleMapvalueType : MapvalueType
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.DoubleMapvalueType";
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class ObjectMapvalueType : MapvalueType
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.ObjectMapvalueType";
      [JsonPropertyName("type")]
      [NewtonsoftJson.JsonProperty("type")]
      public TypeIdentifier type { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class RelationshipMapvalueType : MapvalueType
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.RelationshipMapvalueType";
      [JsonPropertyName("type")]
      [NewtonsoftJson.JsonProperty("type")]
      public TypeIdentifier type { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class EnumDeclaration : Declaration
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.EnumDeclaration";
      [JsonPropertyName("properties")]
      [NewtonsoftJson.JsonProperty("properties")]
      public EnumProperty[] properties { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class EnumProperty : Concept
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.EnumProperty";
      [System.ComponentModel.DataAnnotations.RegularExpression(@"^(\p{Lu}|\p{Ll}|\p{Lt}|\p{Lm}|\p{Lo}|\p{Nl}|\$|_|\\u[0-9A-Fa-f]{4})(?:\p{Lu}|\p{Ll}|\p{Lt}|\p{Lm}|\p{Lo}|\p{Nl}|\$|_|\\u[0-9A-Fa-f]{4}|\p{Mn}|\p{Mc}|\p{Nd}|\p{Pc}|\u200C|\u200D)*$", ErrorMessage = "Invalid characters")]
      [JsonPropertyName("name")]
      [NewtonsoftJson.JsonProperty("name")]
      public string name { get; set; }
      [JsonPropertyName("decorators")]
      [NewtonsoftJson.JsonProperty("decorators")]
      public Decorator?[] decorators  { get; set; }
      [JsonPropertyName("location")]
      [NewtonsoftJson.JsonProperty("location")]
      public Range? location { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class ConceptDeclaration : Declaration
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.ConceptDeclaration";
      [JsonPropertyName("isAbstract")]
      [NewtonsoftJson.JsonProperty("isAbstract")]
      public bool isAbstract { get; set; }
      [JsonPropertyName("identified")]
      [NewtonsoftJson.JsonProperty("identified")]
      public identified? identified { get; set; }
      [JsonPropertyName("superType")]
      [NewtonsoftJson.JsonProperty("superType")]
      public TypeIdentifier? superType{ get; set; }
      [JsonPropertyName("properties")]
      [NewtonsoftJson.JsonProperty("properties")]
      public Property[] properties { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class AssetDeclaration : ConceptDeclaration
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.AssetDeclaration";
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class ParticipantDeclaration : ConceptDeclaration
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.ParticipantDeclaration";
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class TransactionDeclaration : ConceptDeclaration
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.TransactionDeclaration";
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class EventDeclaration : ConceptDeclaration
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.EventDeclaration";
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public abstract class Property : Concept
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.Property";
      [System.ComponentModel.DataAnnotations.RegularExpression(@"^(\p{Lu}|\p{Ll}|\p{Lt}|\p{Lm}|\p{Lo}|\p{Nl}|\$|_|\\u[0-9A-Fa-f]{4})(?:\p{Lu}|\p{Ll}|\p{Lt}|\p{Lm}|\p{Lo}|\p{Nl}|\$|_|\\u[0-9A-Fa-f]{4}|\p{Mn}|\p{Mc}|\p{Nd}|\p{Pc}|\u200C|\u200D)*$", ErrorMessage = "Invalid characters")]
      [JsonPropertyName("name")]
      [NewtonsoftJson.JsonProperty("name")]
      public string name { get; set; }
      [JsonPropertyName("isArray")]
      [NewtonsoftJson.JsonProperty("isArray")]
      public bool isArray { get; set; }
      [JsonPropertyName("isOptional")]
      [NewtonsoftJson.JsonProperty("isOptional")]
      public bool  isOptional    { get; set; }
      [JsonPropertyName("decorators")]
      [NewtonsoftJson.JsonProperty("decorators")]
      public Decorator?[] decorators  { get; set; }
      [JsonPropertyName("location")]
      [NewtonsoftJson.JsonProperty("location")]
      public Range? location { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class RelationshipProperty : Property
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.RelationshipProperty";
      [JsonPropertyName("type")]
      [NewtonsoftJson.JsonProperty("type")]
      public TypeIdentifier type { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class ObjectProperty : Property
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.ObjectProperty";
      [JsonPropertyName("defaultvalue")]
      [NewtonsoftJson.JsonProperty("defaultvalue")]
      public string? defaultValue { get; set; }
      [JsonPropertyName("type")]
      [NewtonsoftJson.JsonProperty("type")]
      public TypeIdentifier type { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class BooleanProperty : Property
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.BooleanProperty";
      [JsonPropertyName("defaultvalue")]
      [NewtonsoftJson.JsonProperty("defaultvalue")]
      public bool? defaultValue { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class DateTimeProperty : Property
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.DateTimeProperty";
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class StringProperty : Property
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.StringProperty";
      [JsonPropertyName("defaultvalue")]
      [NewtonsoftJson.JsonProperty("defaultvalue")]
      public string? defaultValue { get; set; }
      [JsonPropertyName("validator")]
      [NewtonsoftJson.JsonProperty("validator")]
      public StringRegexValidator? validator { get; set; }
      [JsonPropertyName("lengthValidator")]
      [NewtonsoftJson.JsonProperty("lengthValidator")]
      public StringLengthValidator? LengthValidator { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class StringRegexValidator : Concept
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.StringRegexValidator";
      [JsonPropertyName("pattern")]
      [NewtonsoftJson.JsonProperty("pattern")]
      public string  pattern { get; set; }
      [JsonPropertyName("flags")]
      [NewtonsoftJson.JsonProperty("flags")]
      public string  flags { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class StringLengthValidator : Concept
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.StringLengthValidator";
      [JsonPropertyName("minLength")]
      [NewtonsoftJson.JsonProperty("minLength")]
      public int? MinLength { get; set; }
      [JsonPropertyName("maxLength")]
      [NewtonsoftJson.JsonProperty("maxLength")]
      public int? MaxLength { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class DoubleProperty : Property
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.DoubleProperty";
      [JsonPropertyName("defaultvalue")]
      [NewtonsoftJson.JsonProperty("defaultvalue")]
      public float? defaultValue { get; set; }
      [JsonPropertyName("validator")]
      [NewtonsoftJson.JsonProperty("validator")]
      public DoubleDomainValidator? validator { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class DoubleDomainValidator : Concept
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.DoubleDomainValidator";
      [JsonPropertyName("lower")]
      [NewtonsoftJson.JsonProperty("lower")]
      public float?  lower  { get; set; }
      [JsonPropertyName("upper")]
      [NewtonsoftJson.JsonProperty("upper")]
      public float?  upper   { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class IntegerProperty : Property
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.IntegerProperty";
      [JsonPropertyName("defaultvalue")]
      [NewtonsoftJson.JsonProperty("defaultvalue")]
      public int? defaultValue { get; set; }
      [JsonPropertyName("validator")]
      [NewtonsoftJson.JsonProperty("validator")]
      public IntegerDomainValidator? validator { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class IntegerDomainValidator : Concept
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.IntegerDomainValidator";
      [JsonPropertyName("lower")]
      [NewtonsoftJson.JsonProperty("lower")]
      public int?  lower  { get; set; }
      [JsonPropertyName("upper")]
      [NewtonsoftJson.JsonProperty("upper")]
      public int?  upper   { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class LongProperty : Property
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.LongProperty";
      [JsonPropertyName("defaultvalue")]
      [NewtonsoftJson.JsonProperty("defaultvalue")]
      public long? defaultValue { get; set; }
      [JsonPropertyName("validator")]
      [NewtonsoftJson.JsonProperty("validator")]
      public LongDomainValidator? validator { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class LongDomainValidator : Concept
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.LongDomainValidator";
      [JsonPropertyName("lower")]
      [NewtonsoftJson.JsonProperty("lower")]
      public long?  lower  { get; set; }
      [JsonPropertyName("upper")]
      [NewtonsoftJson.JsonProperty("upper")]
      public long?  upper   { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public abstract class Import : Concept
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.Import";
      [JsonPropertyName("namespace")]
      [NewtonsoftJson.JsonProperty("namespace")]
      public string _namespace  { get; set; }
      [JsonPropertyName("uri")]
      [NewtonsoftJson.JsonProperty("uri")]
      public string?  uri    { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class ImportAll : Import
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.ImportAll";
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class ImportType : Import
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.ImportType";
      [JsonPropertyName("name")]
      [NewtonsoftJson.JsonProperty("name")]
      public string name { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class ImportTypes : Import
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.ImportTypes";
      [JsonPropertyName("types")]
      [NewtonsoftJson.JsonProperty("types")]
      public string[] Types { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class Model : Concept
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.Model";
      [JsonPropertyName("namespace")]
      [NewtonsoftJson.JsonProperty("namespace")]
      public string _namespace  { get; set; }
      [JsonPropertyName("sourceUri")]
      [NewtonsoftJson.JsonProperty("sourceUri")]
      public string? sourceUri      { get; set; }
      [JsonPropertyName("concertoVersion")]
      [NewtonsoftJson.JsonProperty("concertoVersion")]
      public string? concertoVersion      { get; set; }
      [JsonPropertyName("imports")]
      [NewtonsoftJson.JsonProperty("imports")]
      public Import?[] imports       { get; set; }
      [JsonPropertyName("declarations")]
      [NewtonsoftJson.JsonProperty("declarations")]
      public Declaration?[] declarations { get; set; }
      [JsonPropertyName("decorators")]
      [NewtonsoftJson.JsonProperty("decorators")]
      public Decorator?[] decorators  { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class Models : Concept
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.Models";
      [JsonPropertyName("models")]
      [NewtonsoftJson.JsonProperty("models")]
      public Model[] _Models { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public abstract class ScalarDeclaration : Declaration
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.ScalarDeclaration";
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class BooleanScalar : ScalarDeclaration
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.BooleanScalar";
      [JsonPropertyName("defaultvalue")]
      [NewtonsoftJson.JsonProperty("defaultvalue")]
      public bool? defaultValue { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class IntegerScalar : ScalarDeclaration
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.IntegerScalar";
      [JsonPropertyName("defaultvalue")]
      [NewtonsoftJson.JsonProperty("defaultvalue")]
      public int? defaultValue { get; set; }
      [JsonPropertyName("validator")]
      [NewtonsoftJson.JsonProperty("validator")]
      public IntegerDomainValidator? validator { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class LongScalar : ScalarDeclaration
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.LongScalar";
      [JsonPropertyName("defaultvalue")]
      [NewtonsoftJson.JsonProperty("defaultvalue")]
      public long? defaultValue { get; set; }
      [JsonPropertyName("validator")]
      [NewtonsoftJson.JsonProperty("validator")]
      public LongDomainValidator? validator { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class DoubleScalar : ScalarDeclaration
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.DoubleScalar";
      [JsonPropertyName("defaultvalue")]
      [NewtonsoftJson.JsonProperty("defaultvalue")]
      public float? defaultValue { get; set; }
      [JsonPropertyName("validator")]
      [NewtonsoftJson.JsonProperty("validator")]
      public DoubleDomainValidator? validator { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class StringScalar : ScalarDeclaration
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.StringScalar";
      [JsonPropertyName("defaultvalue")]
      [NewtonsoftJson.JsonProperty("defaultvalue")]
      public string? defaultValue { get; set; }
      [JsonPropertyName("validator")]
      [NewtonsoftJson.JsonProperty("validator")]
      public StringRegexValidator? validator { get; set; }
      [JsonPropertyName("lengthValidator")]
      [NewtonsoftJson.JsonProperty("lengthValidator")]
      public StringLengthValidator? lengthValidator { get; set; }
   }
   [NewtonsoftJson.JsonConverter(typeof(NewtonsoftConcerto.ConcertoConverter))]
   public class DateTimeScalar : ScalarDeclaration
   {
      [JsonPropertyName("$class")]
      [NewtonsoftJson.JsonProperty("$class")]
      public override string _class { get; } = "concerto.metamodel@1.0.0.DateTimeScalar";
      [JsonPropertyName("defaultvalue")]
      [NewtonsoftJson.JsonProperty("defaultvalue")]
      public string? defaultValue { get; set; }
   }
}
