using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class DrugControllerRequest
    {
        [JsonProperty("names")]
        public List<string> Names { get; set; }
    }
}
