using Newtonsoft.Json;

namespace TestTask_A2_Parser.Responses
{
    public class ResponseData
    {
        [JsonProperty("data")]
        public ReportWoodDeal Data { get; set; }
    }
}