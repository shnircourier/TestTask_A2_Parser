using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using Shared.Entities;

namespace Data
{
    public static class WoodDealDataContext
    {
        private const string ConnectionString = "Data Source=.;Initial Catalog=TestTask_Parser_A2;Integrated Security=True;";

        public static HashSet<string> GetAllKeys()
        {
            var keys = new HashSet<string>();
            
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                
                var query = "SELECT DealNumber FROM WoodDeal";
                var command = new SqlCommand(query, connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        keys.Add(reader["DealNumber"].ToString());
                    }    
                }
            }

            return keys;
        }

        public static void InsertBatch(List<WoodDeal> woodDeals)
        {
            var raw =
                "INSERT INTO dbo.WoodDeal " +
                "(DealNumber," +
                " SellerName, " +
                "SellerInn, " +
                "BuyerName, " +
                "BuyerInn, " +
                "DealDate, " +
                "WoodVolumeSeller, " +
                "WoodVolumeBuyer)" +
                "VALUES ";

            woodDeals.ForEach(w =>
                raw +=
                    $"(N'{w.DealNumber}'," +
                    $" N'{w.SellerName?.Replace('\'', '\"')}'," +
                    $" N'{w.SellerInn}'," +
                    $" N'{w.BuyerName?.Replace('\'', '\"')}'," +
                    $" N'{w.BuyerInn}'," +
                    $" '{w.DealDate ?? null}'," +
                    $" {w.WoodVolumeSeller.ToString(CultureInfo.InvariantCulture)}," +
                    $" {w.WoodVolumeBuyer.ToString(CultureInfo.InvariantCulture)}),");

            var query = raw.Remove(raw.Length - 1, 1);

            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand(query, connection);
                var count = command.ExecuteNonQuery();
            }
        }

        public static void UpdateBatch(List<WoodDeal> woodDeals)
        {
            foreach (var w in woodDeals)
            {
                var woodDeal = FindByDealNumber(w.DealNumber);

                if (IsUpdateNeed(w, woodDeal))
                {
                    UpdateRow(woodDeal);
                }
            }
        }

        private static WoodDeal FindByDealNumber(string dealNumber)
        {
            var query = $"SELECT TOP 1 * FROM WoodDeal WHERE DealNumber = '{dealNumber}'";
            var woodDeal = new WoodDeal();
            
            
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand(query, connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        woodDeal.BuyerInn = reader["BuyerInn"].ToString();
                        woodDeal.BuyerName = reader["BuyerName"].ToString();
                        woodDeal.DealDate = DateTime.Parse(reader["DealDate"].ToString());
                        woodDeal.DealNumber = reader["DealNumber"].ToString();
                        woodDeal.SellerInn = reader["SellerInn"].ToString();
                        woodDeal.SellerName = reader["SellerName"].ToString();
                        woodDeal.WoodVolumeBuyer = double.Parse(reader["WoodVolumeBuyer"].ToString());
                        woodDeal.WoodVolumeSeller = double.Parse(reader["WoodVolumeSeller"].ToString());
                    }
                }
            }

            return woodDeal;
        }

        private static bool IsUpdateNeed(WoodDeal fromHttp, WoodDeal fromDb)
        {
            if (fromHttp.DealDate > fromDb.DealDate)
            {
                return true;
            }

            if (fromHttp.WoodVolumeBuyer != fromDb.WoodVolumeBuyer && fromDb.WoodVolumeBuyer == 0)
            {
                return true;
            }

            if (fromHttp.WoodVolumeSeller != fromDb.WoodVolumeBuyer && fromDb.WoodVolumeSeller == 0)
            {
                return true;
            }

            if (fromDb.BuyerInn is null || !fromDb.Equals(fromHttp.BuyerInn))
            {
                return true;
            }

            if (fromDb.BuyerName is null || !fromDb.Equals(fromHttp.BuyerName))
            {
                return true;
            }

            if (fromDb.SellerName is null || !fromDb.Equals(fromHttp.SellerName))
            {
                return true;
            }
            
            if (fromDb.SellerInn is null || !fromDb.Equals(fromHttp.SellerInn))
            {
                return true;
            }

            return false;
        }
        
        private static void UpdateRow(WoodDeal woodDeal)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                
                var query = $"UPDATE WoodDeal SET " +
                            $"BuyerInn = N'{woodDeal.BuyerInn}'," +
                            $"BuyerName = N'{woodDeal.BuyerName?.Replace('\'', '\"')}'," +
                            $"DealDate = N'{woodDeal.DealDate}'," +
                            $"SellerInn = N'{woodDeal.SellerInn}'," +
                            $"SellerName = N'{woodDeal.SellerName?.Replace('\'', '\"')}'," +
                            $"WoodVolumeBuyer = {woodDeal.WoodVolumeBuyer.ToString(CultureInfo.InvariantCulture)}," +
                            $"WoodVolumeSeller = {woodDeal.WoodVolumeSeller.ToString(CultureInfo.InvariantCulture)} " +
                            $"WHERE DealNumber = N'{woodDeal.DealNumber}'";
        
                var command = new SqlCommand(query, connection);
                var count = command.ExecuteNonQuery();
            }
        }
    }
}