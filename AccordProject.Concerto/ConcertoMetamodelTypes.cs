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

using System;
using System.Text.Json.Serialization;

namespace AccordProject.Concerto.Metamodel {
   using AccordProject.Concerto;
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "Position")]
   public class Position : Concept {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.Position";
      public int line { get; set; }
      public int column { get; set; }
      public int offset { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "Range")]
   public class Range : Concept {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.Range";
      public Position start { get; set; }
      public Position end { get; set; }
      public string source { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "TypeIdentifier")]
   public class TypeIdentifier : Concept {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.TypeIdentifier";
      public string name { get; set; }
      [JsonPropertyName("namespace")]
		public string _namespace { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "DecoratorLiteral")]
   public abstract class DecoratorLiteral : Concept {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.DecoratorLiteral";
      public Range location { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "DecoratorString")]
   public class DecoratorString : DecoratorLiteral {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.DecoratorString";
      public string value { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "DecoratorNumber")]
   public class DecoratorNumber : DecoratorLiteral {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.DecoratorNumber";
      public float value { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "DecoratorBoolean")]
   public class DecoratorBoolean : DecoratorLiteral {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.DecoratorBoolean";
      public bool value { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "DecoratorTypeReference")]
   public class DecoratorTypeReference : DecoratorLiteral {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.DecoratorTypeReference";
      public TypeIdentifier type { get; set; }
      public bool isArray { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "Decorator")]
   public class Decorator : Concept {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.Decorator";
      public string name { get; set; }
      public DecoratorLiteral[] arguments { get; set; }
      public Range location { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "Identified")]
   public class Identified : Concept {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.Identified";
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "IdentifiedBy")]
   public class IdentifiedBy : Identified {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.IdentifiedBy";
      public string name { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "Declaration")]
   public abstract class Declaration : Concept {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.Declaration";
      public string name { get; set; }
      public Decorator[] decorators { get; set; }
      public Range location { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "EnumDeclaration")]
   public class EnumDeclaration : Declaration {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.EnumDeclaration";
      public EnumProperty[] properties { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "EnumProperty")]
   public class EnumProperty : Concept {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.EnumProperty";
      public string name { get; set; }
      public Decorator[] decorators { get; set; }
      public Range location { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "ConceptDeclaration")]
   public class ConceptDeclaration : Declaration {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.ConceptDeclaration";
      public bool isAbstract { get; set; }
      public Identified identified { get; set; }
      public TypeIdentifier superType { get; set; }
      public Property[] properties { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "AssetDeclaration")]
   public class AssetDeclaration : ConceptDeclaration {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.AssetDeclaration";
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "ParticipantDeclaration")]
   public class ParticipantDeclaration : ConceptDeclaration {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.ParticipantDeclaration";
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "TransactionDeclaration")]
   public class TransactionDeclaration : ConceptDeclaration {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.TransactionDeclaration";
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "EventDeclaration")]
   public class EventDeclaration : ConceptDeclaration {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.EventDeclaration";
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "Property")]
   public abstract class Property : Concept {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.Property";
      public string name { get; set; }
      public bool isArray { get; set; }
      public bool isOptional { get; set; }
      public Decorator[] decorators { get; set; }
      public Range location { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "RelationshipProperty")]
   public class RelationshipProperty : Property {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.RelationshipProperty";
      public TypeIdentifier type { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "ObjectProperty")]
   public class ObjectProperty : Property {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.ObjectProperty";
      public string defaultValue { get; set; }
      public TypeIdentifier type { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "BooleanProperty")]
   public class BooleanProperty : Property {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.BooleanProperty";
      public bool defaultValue { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "DateTimeProperty")]
   public class DateTimeProperty : Property {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.DateTimeProperty";
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "StringProperty")]
   public class StringProperty : Property {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.StringProperty";
      public string defaultValue { get; set; }
      public StringRegexValidator validator { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "StringRegexValidator")]
   public class StringRegexValidator : Concept {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.StringRegexValidator";
      public string pattern { get; set; }
      public string flags { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "DoubleProperty")]
   public class DoubleProperty : Property {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.DoubleProperty";
      public float defaultValue { get; set; }
      public DoubleDomainValidator validator { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "DoubleDomainValidator")]
   public class DoubleDomainValidator : Concept {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.DoubleDomainValidator";
      public float lower { get; set; }
      public float upper { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "IntegerProperty")]
   public class IntegerProperty : Property {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.IntegerProperty";
      public int defaultValue { get; set; }
      public IntegerDomainValidator validator { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "IntegerDomainValidator")]
   public class IntegerDomainValidator : Concept {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.IntegerDomainValidator";
      public int lower { get; set; }
      public int upper { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "LongProperty")]
   public class LongProperty : Property {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.LongProperty";
      public long defaultValue { get; set; }
      public LongDomainValidator validator { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "LongDomainValidator")]
   public class LongDomainValidator : Concept {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.LongDomainValidator";
      public long lower { get; set; }
      public long upper { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "Import")]
   public abstract class Import : Concept {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.Import";
      [JsonPropertyName("namespace")]
		public string _namespace { get; set; }
      public string uri { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "ImportAll")]
   public class ImportAll : Import {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.ImportAll";
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "ImportType")]
   public class ImportType : Import {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.ImportType";
      public string name { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "ImportTypes")]
   public class ImportTypes : Import {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.ImportTypes";
      public string[] types { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "Model")]
   public class Model : Concept {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.Model";
      [JsonPropertyName("namespace")]
		public string _namespace { get; set; }
      public string sourceUri { get; set; }
      public string concertoVersion { get; set; }
      public Import[] imports { get; set; }
      public Declaration[] declarations { get; set; }
      public Decorator[] decorators { get; set; }
   }
   [AccordProject.Concerto.Type(Namespace = "concerto.metamodel", Version = "1.0.0", Name = "Models")]
   public class Models : Concept {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto.metamodel@1.0.0.Models";
      public Model[] models { get; set; }
   }
}
