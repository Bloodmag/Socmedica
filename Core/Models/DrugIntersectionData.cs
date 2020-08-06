using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Core.Models
{
    public class DrugIntersectionData
    {
        [JsonProperty("names")]
        public List<string> Names { get; set; }
        [JsonProperty("intersections")]
        public List<string> Intersections { get; set; }
    }
}
