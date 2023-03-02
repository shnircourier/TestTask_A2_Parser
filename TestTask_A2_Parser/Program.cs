using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using Data;
using Newtonsoft.Json;
using Shared.Entities;
using TestTask_A2_Parser.Requests;
using TestTask_A2_Parser.Responses;
using Timer = System.Timers.Timer;

namespace TestTask_A2_Parser
{
    internal class Program
    {
        private const string Url = "https://www.lesegais.ru/open-area/graphql";
        private const int Size = 1_000;

        public static void Main(string[] args)
        {
            while (true)
            {
                var dealNumbers = WoodDealDataContext.GetAllKeys();

                var pageCount = Math.Ceiling((double)GetRowsCount() / Size);

                for (var i = 0; i < (int) pageCount; i++)
                {
                    var dataFromHttp = GetDataFromHttpRequest(i);
                    var dataToAdd = new List<WoodDeal>();
                    var dataToUpdate = new List<WoodDeal>();
                
                    dataFromHttp.ForEach(d =>
                    {
                        if (!dealNumbers.Contains(d.DealNumber))
                        {
                            dealNumbers.Add(d.DealNumber);
                            dataToAdd.Add(d);
                        }
                        else
                        {
                            dataToUpdate.Add(d);
                        }
                    });

                    if (dataToAdd.Count != 0)
                    {
                        WoodDealDataContext.InsertBatch(dataToAdd);
                    }

                    if (dataToUpdate.Count != 0)
                    {
                        WoodDealDataContext.UpdateBatch(dataToUpdate);
                    }
                }
                
                Thread.Sleep(600_000);
            }
        }

        static List<WoodDeal> GetDataFromHttpRequest(int page)
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
                        Number = page,
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