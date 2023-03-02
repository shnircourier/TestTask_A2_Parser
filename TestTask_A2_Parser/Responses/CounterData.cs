using Newtonsoft.Json;

namespace TestTask_A2_Parser.Responses
{
    public class CounterData
    {
        [JsonProperty("total")]
        public int Total { get; set; }
    }
}