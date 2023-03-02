using Newtonsoft.Json;

namespace TestTask_A2_Parser.Requests
{
    public class Variables
    {
        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("number")]
        public int Number { get; set; }
    }
}