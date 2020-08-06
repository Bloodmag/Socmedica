using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Core.Models
{
    public class DrugControllerResponse
    {
        [JsonProperty("drug_intersections")]
        public List<DrugIntersectionData> DrugIntersections { get; set; }
        [JsonProperty("error_message")]
        public string ErrorMessage { get; set; }
    }
}
