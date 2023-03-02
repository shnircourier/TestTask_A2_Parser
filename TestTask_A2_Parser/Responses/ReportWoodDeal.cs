using System.Collections.Generic;
using Newtonsoft.Json;

namespace TestTask_A2_Parser.Responses
{
    public class ReportWoodDeal
    {
        [JsonProperty("searchReportWoodDeal")]
        public Content SearchReportWoodDeal { get; set; }
    }
}