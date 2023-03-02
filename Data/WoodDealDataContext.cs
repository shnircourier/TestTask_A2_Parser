using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using Shared.Entities;

namespace Data
{
    public static class WoodDealDataContext
    {
        private const string ConnectionString = "Data Source=.;Initial Catalog=TestTask_Parser_A2;Integrated Security=True;";
        
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
                    $" N'{w.SellerName}'," +
                    $" N'{w.SellerInn}'," +
                    $" N'{w.BuyerName}'," +
                    $" N'{w.BuyerInn}'," +
                    $" '{w.DealDate}'," +
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
    }
}