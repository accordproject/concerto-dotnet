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

namespace AccordProject.Concerto;
using Concerto.Decorator;
[AccordProject.Concerto.Type(Namespace = "concerto", Version = "1.0.0", Name = "Concept")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public abstract class Concept {
   [Newtonsoft.Json.JsonProperty("$class")]
   public virtual string _Class { get; } = "concerto@1.0.0.Concept";
}
[AccordProject.Concerto.Type(Namespace = "concerto", Version = "1.0.0", Name = "Asset")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public abstract class Asset : Concept {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto@1.0.0.Asset";
   [AccordProject.Concerto.Identifier()]
   [Newtonsoft.Json.JsonProperty("$identifier")]
   public string _Identifier { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto", Version = "1.0.0", Name = "Participant")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public abstract class Participant : Concept {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto@1.0.0.Participant";
   [AccordProject.Concerto.Identifier()]
   [Newtonsoft.Json.JsonProperty("$identifier")]
   public string _Identifier { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto", Version = "1.0.0", Name = "Transaction")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public abstract class Transaction : Concept {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto@1.0.0.Transaction";
   [Newtonsoft.Json.JsonProperty("$timestamp")]
   public System.DateTime _Timestamp { get; set; }
}
[AccordProject.Concerto.Type(Namespace = "concerto", Version = "1.0.0", Name = "Event")]
[Newtonsoft.Json.JsonConverter(typeof(AccordProject.Concerto.ConcertoConverterNewtonsoft))]
public abstract class Event : Concept {
   [Newtonsoft.Json.JsonProperty("$class")]
   public override string _Class { get; } = "concerto@1.0.0.Event";
   [Newtonsoft.Json.JsonProperty("$timestamp")]
   public System.DateTime _Timestamp { get; set; }
}
