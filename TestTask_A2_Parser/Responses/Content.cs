using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Entities;

namespace TestTask_A2_Parser.Responses
{
    public class Content
    {
        [JsonProperty("content")]
        public List<WoodDeal> ContentData { get; set; }
        
        [JsonProperty("total")]
        public int Total { get; set; } 
    }
}