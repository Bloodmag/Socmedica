using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class ApiResponseData
    {
        [JsonProperty("alerts")]
        public ApiAlert Alert { get; set; }
        [JsonProperty("graph")]
        public List<Graph> Graph { get; set; }
        /// <summary>
        /// Graph.Id, Graph node's name
        /// </summary>
        [JsonProperty("names")]
        public Dictionary<uint,string> Names { get; set; }
        /// <summary>
        /// Graph.Type, Type's name 
        /// </summary>
        [JsonProperty("levels")]
        public Dictionary<short,string> Levels { get; set; }
        /// <summary>
        /// Graph.Id, Graph node's index
        /// </summary>
        [JsonProperty("keygraphs")]
        public Dictionary<uint,uint> Keygraph { get; set; }
    }
}
