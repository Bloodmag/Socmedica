using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class Graph
    {
        [JsonProperty("id")]
        public uint Id { get; set; }
        [JsonProperty("type")]
        public short Type { get; set; }
        [JsonProperty("deep")]
        public short Deepness { get; set; }
        [JsonProperty("level")]
        public ushort Level { get; set; }
        [JsonProperty("route")]
        public short Route { get; set; }
        [JsonProperty("ida")]
        public uint IdA { get; set; }
        [JsonProperty("levela")]
        public ushort LevelA { get; set; }
        [JsonProperty("idb")]
        public uint IdB { get; set; }
        [JsonProperty("levelb")]
        public ushort LevelB { get; set; }
        [JsonProperty("value_a")]
        public double? ValueA { get; set; }
        [JsonProperty("value_b")]
        public double? ValueB { get; set; }
        [JsonProperty("value_c")]
        public double? ValueC { get; set; }
        [JsonProperty("value_d")]
        public double? ValueD { get; set; }
        [JsonProperty("sort")]
        public uint? Sort { get; set; }
    }
}
