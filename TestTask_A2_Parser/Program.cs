using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Data;
using Newtonsoft.Json;
using Shared.Entities;
using TestTask_A2_Parser.Requests;
using TestTask_A2_Parser.Responses;

namespace TestTask_A2_Parser
{
    internal class Program
    {
        private const string Url = "https://www.lesegais.ru/open-area/graphql";
        private const int Size = 1_000;

        public static void Main(string[] args)
        {
            //var data = GetDataFromHttpRequest();
            var count = GetRowsCount();
        }

        static List<WoodDeal> GetDataFromHttpRequest()
        {
            using (var client = new HttpClient())
            {
                var payloadObject = new Payload
                {
                    OperationName = "SearchReportWoodDeal",
                    Query =
                        "query SearchReportWoodDeal($size: Int!, $number: Int!, $filter: Filter, $orders: [Order!]) {\n  searchReportWoodDeal(filter: $filter, pageable: {number: $number, size: $size}, orders: $orders) {\n    content {\n      sellerName\n      sellerInn\n      buyerName\n      buyerInn\n      woodVolumeBuyer\n      woodVolumeSeller\n      dealDate\n      dealNumber\n      __typename\n    }\n    __typename\n  }\n}\n",
                    Variables = new Variables
                    {
                        Number = 0,
                        Size = Size
                    }
                };

                var payloadData = JsonConvert.SerializeObject(payloadObject);

                var payload = new StringContent(payloadData, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Add("User-Agent", "Parser");

                var result = client.PostAsync(Url, payload)
                    .Result
                    .Content
                    .ReadAsStringAsync()
                    .Result;

                var data = JsonConvert.DeserializeObject<ResponseData>(result)?.Data.SearchReportWoodDeal.ContentData;

                return data;
            }
        }

        static int GetRowsCount()
        {
            using (var client = new HttpClient())
            {
                var payloadObject = new Payload
                {
                    OperationName = "SearchReportWoodDealCount",
                    Query =
                        "query SearchReportWoodDealCount($size: Int!, $number: Int!, $filter: Filter, $orders: [Order!]) {\n  searchReportWoodDeal(filter: $filter, pageable: {number: $number, size: $size}, orders: $orders) {\n    total\n    number\n    size\n    overallBuyerVolume\n    overallSellerVolume\n    __typename\n  }\n}\n",
                    Variables = new Variables
                    {
                        Number = 0,
                        Size = 20
                    }
                };

                var payloadData = JsonConvert.SerializeObject(payloadObject);

                var payload = new StringContent(payloadData, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Add("User-Agent", "Parser");

                var result = client.PostAsync(Url, payload)
                    .Result
                    .Content
                    .ReadAsStringAsync()
                    .Result;

                var data = JsonConvert.DeserializeObject<ResponseData>(result).Data.SearchReportWoodDeal.Total;

                return data;
            }
        }
    }
}