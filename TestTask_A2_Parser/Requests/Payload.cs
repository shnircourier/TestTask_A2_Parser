using Newtonsoft.Json;

namespace TestTask_A2_Parser.Requests
{
    public class Payload
    {
        [JsonProperty("operationName")]
        public string OperationName { get; set; }

        [JsonProperty("query")]
        public string Query { get; set; }

        [JsonProperty("variables")]
        public Variables Variables { get; set; }
    }
}