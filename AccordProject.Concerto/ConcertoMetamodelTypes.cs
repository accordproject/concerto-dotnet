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

namespace AccordProject.Concerto.Metamodel;
using AccordProject.Concerto;
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "Position")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class Position : Concept {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.Position";
   [Newtonsoft.Json.JsonProperty("line")]
   public int Line { get; set; }
   [Newtonsoft.Json.JsonProperty("column")]
   public int Column { get; set; }
   [Newtonsoft.Json.JsonProperty("offset")]
   public int Offset { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "Range")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class Range : Concept {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.Range";
   [Newtonsoft.Json.JsonProperty("start")]
   public Position Start { get; set; }
   [Newtonsoft.Json.JsonProperty("end")]
   public Position End { get; set; }
   [Newtonsoft.Json.JsonProperty("source")]
   public string? Source { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "TypeIdentifier")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class TypeIdentifier : Concept {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.TypeIdentifier";
   [Newtonsoft.Json.JsonProperty("name")]
   public string Name { get; set; }
   [Newtonsoft.Json.JsonProperty("namespace")]
   public string? Namespace { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "DecoratorLiteral")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public abstract class DecoratorLiteral : Concept {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.DecoratorLiteral";
   [Newtonsoft.Json.JsonProperty("location")]
   public Range? Location { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "DecoratorString")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class DecoratorString : DecoratorLiteral {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.DecoratorString";
   [Newtonsoft.Json.JsonProperty("value")]
   public string Value { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "DecoratorNumber")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class DecoratorNumber : DecoratorLiteral {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.DecoratorNumber";
   [Newtonsoft.Json.JsonProperty("value")]
   public float Value { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "DecoratorBoolean")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class DecoratorBoolean : DecoratorLiteral {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.DecoratorBoolean";
   [Newtonsoft.Json.JsonProperty("value")]
   public bool Value { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "DecoratorTypeReference")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class DecoratorTypeReference : DecoratorLiteral {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.DecoratorTypeReference";
   [Newtonsoft.Json.JsonProperty("type")]
   public TypeIdentifier Type { get; set; }
   [Newtonsoft.Json.JsonProperty("isArray")]
   public bool IsArray { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "Decorator")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class Decorator : Concept {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.Decorator";
   [Newtonsoft.Json.JsonProperty("name")]
   public string Name { get; set; }
   [Newtonsoft.Json.JsonProperty("arguments")]
   public DecoratorLiteral?[] Arguments { get; set; }
   [Newtonsoft.Json.JsonProperty("location")]
   public Range? Location { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "Identified")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class Identified : Concept {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.Identified";
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "IdentifiedBy")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class IdentifiedBy : Identified {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.IdentifiedBy";
   [Newtonsoft.Json.JsonProperty("name")]
   public string Name { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "Declaration")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public abstract class Declaration : Concept {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.Declaration";
   [System.ComponentModel.DataAnnotations.RegularExpression(@"^(\p{Lu}|\p{Ll}|\p{Lt}|\p{Lm}|\p{Lo}|\p{Nl}|\$|_|\\u[0-9A-Fa-f]{4})(?:\p{Lu}|\p{Ll}|\p{Lt}|\p{Lm}|\p{Lo}|\p{Nl}|\$|_|\\u[0-9A-Fa-f]{4}|\p{Mn}|\p{Mc}|\p{Nd}|\p{Pc}|\u200C|\u200D)*$", ErrorMessage = "Invalid characters")]
   [Newtonsoft.Json.JsonProperty("name")]
   public string Name { get; set; }
   [Newtonsoft.Json.JsonProperty("decorators")]
   public Decorator?[] Decorators { get; set; }
   [Newtonsoft.Json.JsonProperty("location")]
   public Range? Location { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "MapKeyType")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public abstract class MapKeyType : Concept {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.MapKeyType";
   [Newtonsoft.Json.JsonProperty("decorators")]
   public Decorator?[] Decorators { get; set; }
   [Newtonsoft.Json.JsonProperty("location")]
   public Range? Location { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "MapValueType")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public abstract class MapValueType : Concept {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.MapValueType";
   [Newtonsoft.Json.JsonProperty("decorators")]
   public Decorator?[] Decorators { get; set; }
   [Newtonsoft.Json.JsonProperty("location")]
   public Range? Location { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "MapDeclaration")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class MapDeclaration : Declaration {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.MapDeclaration";
   [Newtonsoft.Json.JsonProperty("key")]
   public MapKeyType Key { get; set; }
   [Newtonsoft.Json.JsonProperty("value")]
   public MapValueType Value { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "StringMapKeyType")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class StringMapKeyType : MapKeyType {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.StringMapKeyType";
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "DateTimeMapKeyType")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class DateTimeMapKeyType : MapKeyType {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.DateTimeMapKeyType";
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "ObjectMapKeyType")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class ObjectMapKeyType : MapKeyType {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.ObjectMapKeyType";
   [Newtonsoft.Json.JsonProperty("type")]
   public TypeIdentifier Type { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "BooleanMapValueType")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class BooleanMapValueType : MapValueType {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.BooleanMapValueType";
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "DateTimeMapValueType")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class DateTimeMapValueType : MapValueType {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.DateTimeMapValueType";
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "StringMapValueType")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class StringMapValueType : MapValueType {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.StringMapValueType";
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "IntegerMapValueType")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class IntegerMapValueType : MapValueType {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.IntegerMapValueType";
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "LongMapValueType")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class LongMapValueType : MapValueType {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.LongMapValueType";
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "DoubleMapValueType")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class DoubleMapValueType : MapValueType {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.DoubleMapValueType";
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "ObjectMapValueType")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class ObjectMapValueType : MapValueType {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.ObjectMapValueType";
   [Newtonsoft.Json.JsonProperty("type")]
   public TypeIdentifier Type { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "RelationshipMapValueType")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class RelationshipMapValueType : MapValueType {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.RelationshipMapValueType";
   [Newtonsoft.Json.JsonProperty("type")]
   public TypeIdentifier Type { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "EnumDeclaration")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class EnumDeclaration : Declaration {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.EnumDeclaration";
   [Newtonsoft.Json.JsonProperty("properties")]
   public EnumProperty[] Properties { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "EnumProperty")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class EnumProperty : Concept {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.EnumProperty";
   [System.ComponentModel.DataAnnotations.RegularExpression(@"^(\p{Lu}|\p{Ll}|\p{Lt}|\p{Lm}|\p{Lo}|\p{Nl}|\$|_|\\u[0-9A-Fa-f]{4})(?:\p{Lu}|\p{Ll}|\p{Lt}|\p{Lm}|\p{Lo}|\p{Nl}|\$|_|\\u[0-9A-Fa-f]{4}|\p{Mn}|\p{Mc}|\p{Nd}|\p{Pc}|\u200C|\u200D)*$", ErrorMessage = "Invalid characters")]
   [Newtonsoft.Json.JsonProperty("name")]
   public string Name { get; set; }
   [Newtonsoft.Json.JsonProperty("decorators")]
   public Decorator?[] Decorators { get; set; }
   [Newtonsoft.Json.JsonProperty("location")]
   public Range? Location { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "ConceptDeclaration")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class ConceptDeclaration : Declaration {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.ConceptDeclaration";
   [Newtonsoft.Json.JsonProperty("isAbstract")]
   public bool IsAbstract { get; set; }
   [Newtonsoft.Json.JsonProperty("identified")]
   public Identified? Identified { get; set; }
   [Newtonsoft.Json.JsonProperty("superType")]
   public TypeIdentifier? SuperType { get; set; }
   [Newtonsoft.Json.JsonProperty("properties")]
   public Property[] Properties { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "AssetDeclaration")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class AssetDeclaration : ConceptDeclaration {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.AssetDeclaration";
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "ParticipantDeclaration")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class ParticipantDeclaration : ConceptDeclaration {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.ParticipantDeclaration";
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "TransactionDeclaration")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class TransactionDeclaration : ConceptDeclaration {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.TransactionDeclaration";
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "EventDeclaration")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class EventDeclaration : ConceptDeclaration {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.EventDeclaration";
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "Property")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public abstract class Property : Concept {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.Property";
   [System.ComponentModel.DataAnnotations.RegularExpression(@"^(\p{Lu}|\p{Ll}|\p{Lt}|\p{Lm}|\p{Lo}|\p{Nl}|\$|_|\\u[0-9A-Fa-f]{4})(?:\p{Lu}|\p{Ll}|\p{Lt}|\p{Lm}|\p{Lo}|\p{Nl}|\$|_|\\u[0-9A-Fa-f]{4}|\p{Mn}|\p{Mc}|\p{Nd}|\p{Pc}|\u200C|\u200D)*$", ErrorMessage = "Invalid characters")]
   [Newtonsoft.Json.JsonProperty("name")]
   public string Name { get; set; }
   [Newtonsoft.Json.JsonProperty("isArray")]
   public bool IsArray { get; set; }
   [Newtonsoft.Json.JsonProperty("isOptional")]
   public bool IsOptional { get; set; }
   [Newtonsoft.Json.JsonProperty("decorators")]
   public Decorator?[] Decorators { get; set; }
   [Newtonsoft.Json.JsonProperty("location")]
   public Range? Location { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "RelationshipProperty")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class RelationshipProperty : Property {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.RelationshipProperty";
   [Newtonsoft.Json.JsonProperty("type")]
   public TypeIdentifier Type { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "ObjectProperty")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class ObjectProperty : Property {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.ObjectProperty";
   [Newtonsoft.Json.JsonProperty("defaultValue")]
   public string? DefaultValue { get; set; }
   [Newtonsoft.Json.JsonProperty("type")]
   public TypeIdentifier Type { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "BooleanProperty")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class BooleanProperty : Property {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.BooleanProperty";
   [Newtonsoft.Json.JsonProperty("defaultValue")]
   public bool? DefaultValue { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "DateTimeProperty")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class DateTimeProperty : Property {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.DateTimeProperty";
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "StringProperty")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class StringProperty : Property {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.StringProperty";
   [Newtonsoft.Json.JsonProperty("defaultValue")]
   public string? DefaultValue { get; set; }
   [Newtonsoft.Json.JsonProperty("validator")]
   public StringRegexValidator? Validator { get; set; }
   [Newtonsoft.Json.JsonProperty("lengthValidator")]
   public StringLengthValidator? LengthValidator { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "StringRegexValidator")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class StringRegexValidator : Concept {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.StringRegexValidator";
   [Newtonsoft.Json.JsonProperty("pattern")]
   public string Pattern { get; set; }
   [Newtonsoft.Json.JsonProperty("flags")]
   public string Flags { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "StringLengthValidator")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class StringLengthValidator : Concept {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.StringLengthValidator";
   [Newtonsoft.Json.JsonProperty("minLength")]
   public int? MinLength { get; set; }
   [Newtonsoft.Json.JsonProperty("maxLength")]
   public int? MaxLength { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "DoubleProperty")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class DoubleProperty : Property {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.DoubleProperty";
   [Newtonsoft.Json.JsonProperty("defaultValue")]
   public float? DefaultValue { get; set; }
   [Newtonsoft.Json.JsonProperty("validator")]
   public DoubleDomainValidator? Validator { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "DoubleDomainValidator")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class DoubleDomainValidator : Concept {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.DoubleDomainValidator";
   [Newtonsoft.Json.JsonProperty("lower")]
   public float? Lower { get; set; }
   [Newtonsoft.Json.JsonProperty("upper")]
   public float? Upper { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "IntegerProperty")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class IntegerProperty : Property {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.IntegerProperty";
   [Newtonsoft.Json.JsonProperty("defaultValue")]
   public int? DefaultValue { get; set; }
   [Newtonsoft.Json.JsonProperty("validator")]
   public IntegerDomainValidator? Validator { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "IntegerDomainValidator")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class IntegerDomainValidator : Concept {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.IntegerDomainValidator";
   [Newtonsoft.Json.JsonProperty("lower")]
   public int? Lower { get; set; }
   [Newtonsoft.Json.JsonProperty("upper")]
   public int? Upper { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "LongProperty")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class LongProperty : Property {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.LongProperty";
   [Newtonsoft.Json.JsonProperty("defaultValue")]
   public long? DefaultValue { get; set; }
   [Newtonsoft.Json.JsonProperty("validator")]
   public LongDomainValidator? Validator { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "LongDomainValidator")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class LongDomainValidator : Concept {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.LongDomainValidator";
   [Newtonsoft.Json.JsonProperty("lower")]
   public long? Lower { get; set; }
   [Newtonsoft.Json.JsonProperty("upper")]
   public long? Upper { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "AliasedType")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class AliasedType : Concept {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.AliasedType";
   [Newtonsoft.Json.JsonProperty("name")]
   public string Name { get; set; }
   [Newtonsoft.Json.JsonProperty("aliasedName")]
   public string AliasedName { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "Import")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public abstract class Import : Concept {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.Import";
   [Newtonsoft.Json.JsonProperty("namespace")]
   public string Namespace { get; set; }
   [Newtonsoft.Json.JsonProperty("uri")]
   public string? Uri { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "ImportAll")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class ImportAll : Import {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.ImportAll";
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "ImportType")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class ImportType : Import {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.ImportType";
   [Newtonsoft.Json.JsonProperty("name")]
   public string Name { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "ImportTypes")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class ImportTypes : Import {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.ImportTypes";
   [Newtonsoft.Json.JsonProperty("types")]
   public string[] Types { get; set; }
   [Newtonsoft.Json.JsonProperty("aliasedTypes")]
   public AliasedType?[] AliasedTypes { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "Model")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class Model : Concept {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.Model";
   [Newtonsoft.Json.JsonProperty("namespace")]
   public string Namespace { get; set; }
   [Newtonsoft.Json.JsonProperty("sourceUri")]
   public string? SourceUri { get; set; }
   [Newtonsoft.Json.JsonProperty("concertoVersion")]
   public string? ConcertoVersion { get; set; }
   [Newtonsoft.Json.JsonProperty("imports")]
   public Import?[] Imports { get; set; }
   [Newtonsoft.Json.JsonProperty("declarations")]
   public Declaration?[] Declarations { get; set; }
   [Newtonsoft.Json.JsonProperty("decorators")]
   public Decorator?[] Decorators { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "Models")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class Models : Concept {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.Models";
   [Newtonsoft.Json.JsonProperty("models")]
   public Model[] _Models { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "ScalarDeclaration")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public abstract class ScalarDeclaration : Declaration {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.ScalarDeclaration";
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "BooleanScalar")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class BooleanScalar : ScalarDeclaration {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.BooleanScalar";
   [Newtonsoft.Json.JsonProperty("defaultValue")]
   public bool? DefaultValue { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "IntegerScalar")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class IntegerScalar : ScalarDeclaration {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.IntegerScalar";
   [Newtonsoft.Json.JsonProperty("defaultValue")]
   public int? DefaultValue { get; set; }
   [Newtonsoft.Json.JsonProperty("validator")]
   public IntegerDomainValidator? Validator { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "LongScalar")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class LongScalar : ScalarDeclaration {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.LongScalar";
   [Newtonsoft.Json.JsonProperty("defaultValue")]
   public long? DefaultValue { get; set; }
   [Newtonsoft.Json.JsonProperty("validator")]
   public LongDomainValidator? Validator { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "DoubleScalar")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class DoubleScalar : ScalarDeclaration {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.DoubleScalar";
   [Newtonsoft.Json.JsonProperty("defaultValue")]
   public float? DefaultValue { get; set; }
   [Newtonsoft.Json.JsonProperty("validator")]
   public DoubleDomainValidator? Validator { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "StringScalar")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class StringScalar : ScalarDeclaration {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.StringScalar";
   [Newtonsoft.Json.JsonProperty("defaultValue")]
   public string? DefaultValue { get; set; }
   [Newtonsoft.Json.JsonProperty("validator")]
   public StringRegexValidator? Validator { get; set; }
   [Newtonsoft.Json.JsonProperty("lengthValidator")]
   public StringLengthValidator? LengthValidator { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "DateTimeScalar")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public class DateTimeScalar : ScalarDeclaration {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto.metamodel@1.0.0.DateTimeScalar";
   [Newtonsoft.Json.JsonProperty("defaultValue")]
   public string? DefaultValue { get; set; }
}
