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

namespace AccordProject.Concerto {
   public abstract class Concept {
      [JsonPropertyName("$class")]
		public virtual string _class { get; } = "concerto@1.0.0.Concept";
   }
   public abstract class Asset : Concept {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto@1.0.0.Asset";
      [JsonPropertyName("$identifier")]
		public string _identifier { get; set; }
   }
   public abstract class Participant : Concept {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto@1.0.0.Participant";
      [JsonPropertyName("$identifier")]
		public string _identifier { get; set; }
   }
   public abstract class Transaction : Concept {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto@1.0.0.Transaction";
      [JsonPropertyName("$timestamp")]
		public DateTime _timestamp { get; set; }
   }
   public abstract class Event : Concept {
      [JsonPropertyName("$class")]
		public override string _class { get; } = "concerto@1.0.0.Event";
      [JsonPropertyName("$timestamp")]
		public DateTime _timestamp { get; set; }
   }
}
