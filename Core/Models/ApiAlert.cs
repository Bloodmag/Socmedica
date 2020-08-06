using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class ApiAlert
    {
        [JsonProperty("code")]
        public ushort Code { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("level")]
        public string Level { get; set; }
        [JsonProperty("sticky`")]
        public bool Sticky { get; set; }
    }
}
