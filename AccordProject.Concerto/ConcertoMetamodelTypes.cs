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

namespace AccordProject.Concerto.Metamodel {
   using AccordProject.Concerto;
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "Position")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public class Position : Concept {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.Position";
      public int line { get; set; }
      public int column { get; set; }
      public int offset { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "Range")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public class Range : Concept {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.Range";
      public Position start { get; set; }
      public Position end { get; set; }
      public string source { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "TypeIdentifier")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public class TypeIdentifier : Concept {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.TypeIdentifier";
      public string name { get; set; }
      [Newtonsoft.Json.JsonProperty("namespace")]
		public string _namespace { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "DecoratorLiteral")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public abstract class DecoratorLiteral : Concept {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.DecoratorLiteral";
      public Range location { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "DecoratorString")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public class DecoratorString : DecoratorLiteral {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.DecoratorString";
      public string value { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "DecoratorNumber")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public class DecoratorNumber : DecoratorLiteral {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.DecoratorNumber";
      public float value { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "DecoratorBoolean")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public class DecoratorBoolean : DecoratorLiteral {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.DecoratorBoolean";
      public bool value { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "DecoratorTypeReference")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public class DecoratorTypeReference : DecoratorLiteral {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.DecoratorTypeReference";
      public TypeIdentifier type { get; set; }
      public bool isArray { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "Decorator")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public class Decorator : Concept {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.Decorator";
      public string name { get; set; }
      public DecoratorLiteral[] arguments { get; set; }
      public Range location { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "Identified")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public class Identified : Concept {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.Identified";
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "IdentifiedBy")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public class IdentifiedBy : Identified {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.IdentifiedBy";
      public string name { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "Declaration")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public abstract class Declaration : Concept {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.Declaration";
      public string name { get; set; }
      public Decorator[] decorators { get; set; }
      public Range location { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "EnumDeclaration")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public class EnumDeclaration : Declaration {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.EnumDeclaration";
      public EnumProperty[] properties { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "EnumProperty")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public class EnumProperty : Concept {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.EnumProperty";
      public string name { get; set; }
      public Decorator[] decorators { get; set; }
      public Range location { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "ConceptDeclaration")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public class ConceptDeclaration : Declaration {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.ConceptDeclaration";
      public bool isAbstract { get; set; }
      public Identified identified { get; set; }
      public TypeIdentifier superType { get; set; }
      public Property[] properties { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "AssetDeclaration")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public class AssetDeclaration : ConceptDeclaration {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.AssetDeclaration";
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "ParticipantDeclaration")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public class ParticipantDeclaration : ConceptDeclaration {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.ParticipantDeclaration";
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "TransactionDeclaration")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public class TransactionDeclaration : ConceptDeclaration {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.TransactionDeclaration";
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "EventDeclaration")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public class EventDeclaration : ConceptDeclaration {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.EventDeclaration";
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "Property")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public abstract class Property : Concept {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.Property";
      public string name { get; set; }
      public bool isArray { get; set; }
      public bool isOptional { get; set; }
      public Decorator[] decorators { get; set; }
      public Range location { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "RelationshipProperty")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public class RelationshipProperty : Property {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.RelationshipProperty";
      public TypeIdentifier type { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "ObjectProperty")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public class ObjectProperty : Property {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.ObjectProperty";
      public string defaultValue { get; set; }
      public TypeIdentifier type { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "BooleanProperty")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public class BooleanProperty : Property {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.BooleanProperty";
      public bool defaultValue { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "DateTimeProperty")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public class DateTimeProperty : Property {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.DateTimeProperty";
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "StringProperty")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public class StringProperty : Property {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.StringProperty";
      public string defaultValue { get; set; }
      public StringRegexValidator validator { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "StringRegexValidator")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public class StringRegexValidator : Concept {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.StringRegexValidator";
      public string pattern { get; set; }
      public string flags { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "DoubleProperty")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public class DoubleProperty : Property {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.DoubleProperty";
      public float defaultValue { get; set; }
      public DoubleDomainValidator validator { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "DoubleDomainValidator")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public class DoubleDomainValidator : Concept {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.DoubleDomainValidator";
      public float lower { get; set; }
      public float upper { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "IntegerProperty")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public class IntegerProperty : Property {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.IntegerProperty";
      public int defaultValue { get; set; }
      public IntegerDomainValidator validator { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "IntegerDomainValidator")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public class IntegerDomainValidator : Concept {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.IntegerDomainValidator";
      public int lower { get; set; }
      public int upper { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "LongProperty")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public class LongProperty : Property {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.LongProperty";
      public long defaultValue { get; set; }
      public LongDomainValidator validator { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "LongDomainValidator")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public class LongDomainValidator : Concept {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.LongDomainValidator";
      public long lower { get; set; }
      public long upper { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "Import")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public abstract class Import : Concept {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.Import";
      [Newtonsoft.Json.JsonProperty("namespace")]
		public string _namespace { get; set; }
      public string uri { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "ImportAll")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public class ImportAll : Import {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.ImportAll";
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "ImportType")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public class ImportType : Import {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.ImportType";
      public string name { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "ImportTypes")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public class ImportTypes : Import {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.ImportTypes";
      public string[] types { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "Model")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public class Model : Concept {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.Model";
      [Newtonsoft.Json.JsonProperty("namespace")]
		public string _namespace { get; set; }
      public string sourceUri { get; set; }
      public string concertoVersion { get; set; }
      public Import[] imports { get; set; }
      public Declaration[] declarations { get; set; }
      public Decorator[] decorators { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "Models")]
   [Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
   public class Models : Concept {
      [Newtonsoft.Json.JsonProperty("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.Models";
      public Model[] models { get; set; }
   }
}
