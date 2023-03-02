using Newtonsoft.Json;

namespace TestTask_A2_Parser.Responses
{
    public class Response
    {
        [JsonProperty("data")]
        public ResponseData Data { get; set; }
    }
}