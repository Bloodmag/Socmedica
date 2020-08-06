using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class ApiRequestData
    {
        [JsonProperty("key")]
        public string Key{ get; set; }
        [JsonProperty("login")]
        public string Login{ get; set; }
        [JsonProperty("authkey")]
        public string AuthKey{ get; set; }
        [JsonProperty("text")]
        public string Text{ get; set; }
        [JsonProperty("lib")]
        public List<ushort> Lib { get; set; }
        [JsonProperty("libid")]
        public List<string> LibId { get; set; }
        [JsonProperty("tool")]
        public List<ushort> Tool { get; set; }
        [JsonProperty("level")]
        public List<ushort> Level { get; set; }
        [JsonProperty("route")]
        public short Route { get; set; }
        [JsonProperty("routs")]
        public List<short> Routes { get; set; }
        [JsonProperty("deep")]
        public short Deepness { get; set; }
        [JsonProperty("binarization")]
        public int Binarization { get; set; }
        [JsonProperty("valmore")]
        public bool Valmore { get; set; }
    }
}
