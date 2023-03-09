using DataModel;
using DB.SharedUtils;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Pg.Repository
{
    public class DataProvider
    {
        private readonly DataGenerator _dataGenerator = new DataGenerator();

        public void Init()
        {
            using (var context = new ProductsDbContext())
            {
                context.Database.EnsureCreated();
                context.Database.Migrate();
            }
        }

        public void SeedData(int total = 1000)
        {
            using (var context = new ProductsDbContext())
            {
                var generatedRecords = new List<Record>();
                var dates = _dataGenerator.GenerateDates(total, 2015, 2022);

                var recordId = 1;

                foreach (var transactionId in Enumerable.Range(1, total))
                {
                    var transactionProductsCount = _dataGenerator.GetRandomArrayElement(Enumerable.Range(1, 10).ToArray());
                    var date = dates[transactionId - 1];
                    var store = _dataGenerator.GetRandomArrayElement(_dataGenerator.STORES);

                    foreach (var product in Enumerable.Range(1, transactionProductsCount))
                    {
                        var record = _dataGenerator.GenerateRecord(recordId++, transactionId, store, date);
                        generatedRecords.Add(record);
                    }

                    if (transactionId % 1000 == 0) Console.WriteLine($"{transactionId * 100 / total}% are done...");
                }

                Console.WriteLine($"Start saving data...");
                context.Records.AddRange(generatedRecords);

                context.SaveChanges();
                Console.WriteLine($"Done...");
            }
        }

        public void DeleteAll()
        {
            using (var context = new ProductsDbContext())
            {
                context.Records.ExecuteDelete();
            }
        }

        public IEnumerable<Record> SelectRange(DateTime from, DateTime till)
        {
            using (var context = new ProductsDbContext())
            {
                return context.Records
                    .Where(r => r.Date >= from && r.Date <= till)
                    .ToArray();
            }
        }

        public decimal GetTotalQuantity()
        {
            using (var context = new ProductsDbContext())
            {
                return context.Records.Sum(r => r.Quantity);
            }
        }

        public decimal GetTotalPrice()
        {
            using (var context = new ProductsDbContext())
            {
                return context.Records.ToArray().Aggregate(new decimal(0), (a, b) => a + b.Price * b.Quantity);
            }
        }

        public decimal GetTotalPrice(DateTime from, DateTime till)
        {
            using (var context = new ProductsDbContext())
            {
                return context.Records
                    .Where(r => r.Date >= from && r.Date <= till)
                    .ToArray()
                    .Aggregate(new decimal(0), (a, b) => a + b.Price * b.Quantity);
            }
        }

        public decimal GetTotalQuantity(string store, string product, DateTime from, DateTime till)
        {
            using (var context = new ProductsDbContext())
            {
                return context.Records
                    .Where(r => r.Date >= from && r.Date <= till
                                && r.Store == store
                                && r.Product == product)
                    .Sum(r => r.Quantity);
            }
        }

        public decimal GetTotalQuantity(string product, DateTime from, DateTime till)
        {
            using (var context = new ProductsDbContext())
            {
                return context.Records
                    .Where(r => r.Date >= from && r.Date <= till
                                && r.Product == product)
                    .Sum(r => r.Quantity);
            }
        }

        public IDictionary<string, decimal> GetTotalPriceByStores(DateTime from, DateTime till)
        {
            using (var context = new ProductsDbContext())
            {
                var results = new Dictionary<string, decimal>();

                using (var command = context.Database.GetDbConnection().CreateCommand())
                {
                    var query =
                        "SELECT \"Store\", SUM(\"Price\" * \"Quantity\") AS income " +
                        "FROM public.\"Records\" " +
                        $"WHERE \"Date\" >= '{from.ToString("o")}' AND \"Date\" <= '{till.ToString("o")}' " +
                        "GROUP BY \"Store\";";
                    command.CommandText = query;
                    command.CommandType = CommandType.Text;

                    context.Database.OpenConnection();

                    using (var result = command.ExecuteReader())
                    {
                        while (result.Read())
                        {
                            results.Add(result.GetString("Store"), result.GetDecimal("income"));
                        }
                    }

                    return results;
                }
            }
        }

        public IEnumerable<KeyValuePair<string, int>> GetProductsPurchasedBy2(DateTime from, DateTime till)
        {
            using (var context = new ProductsDbContext())
            {
                var results = new List<KeyValuePair<string, int>>();

                using (var command = context.Database.GetDbConnection().CreateCommand())
                {
                    var query =
                    "WITH order_pairs AS( " +
                        "SELECT (CONCAT(pg1.\"Product\", ', ', pg2.\"Product\")) AS items, pg1.\"TransactionId\"" +
                        "FROM " +
                        "(SELECT DISTINCT \"Product\", \"TransactionId\" " +
                        "FROM public.\"Records\") AS pg1 " +
                        "JOIN " +
                        "(SELECT DISTINCT \"Product\", \"TransactionId\" " +
                        "FROM public.\"Records\") AS pg2 " +
                        "ON " +
                        "(" +
                            "pg1.\"TransactionId\" = pg2.\"TransactionId\" AND " +
                            "pg1.\"Product\" != pg2.\"Product\" AND " +
                            "pg1.\"Product\" < pg2.\"Product\" " +
                        ")" +
                    ")" +

                    "SELECT items, COUNT(*) AS frequency " +
                    "FROM order_pairs " +
                    "GROUP by items " +
                    "ORDER BY frequency DESC " +
                    "LIMIT 10;";

                    command.CommandText = query;
                    command.CommandType = CommandType.Text;

                    context.Database.OpenConnection();

                    using (var result = command.ExecuteReader())
                    {
                        while (result.Read())
                        {
                            results.Add(new KeyValuePair<string, int>(result.GetString("items"), result.GetInt32("frequency")));
                        }
                    }

                    return results;
                }
            }
        }

        public IEnumerable<KeyValuePair<string, int>> GetProductsPurchasedBy3(DateTime from, DateTime till)
        {
            using (var context = new ProductsDbContext())
            {
                var results = new List<KeyValuePair<string, int>>();

                using (var command = context.Database.GetDbConnection().CreateCommand())
                {
                    var query =
                    "WITH order_pairs AS( " +
                        "SELECT (CONCAT(pg1.\"Product\", ', ', pg2.\"Product\", ', ', pg3.\"Product\")) AS items, pg1.\"TransactionId\"" +
                        "FROM " +
                        "(SELECT DISTINCT \"Product\", \"TransactionId\" " +
                        "FROM public.\"Records\") AS pg1 " +
                        "JOIN " +
                        "(SELECT DISTINCT \"Product\", \"TransactionId\" " +
                        "FROM public.\"Records\") AS pg2 " +
                        "ON " +
                        "(" +
                            "pg1.\"TransactionId\" = pg2.\"TransactionId\" AND " +
                            "pg1.\"Product\" != pg2.\"Product\" AND " +
                            "pg1.\"Product\" < pg2.\"Product\" " +
                        ")" +
                        "JOIN " +
                        "(SELECT DISTINCT \"Product\", \"TransactionId\" " +
                        "FROM public.\"Records\") AS pg3 " +
                        "ON " +
                        "(" +
                            "pg1.\"TransactionId\" = pg3.\"TransactionId\" AND " +
                            "pg1.\"Product\" != pg3.\"Product\" AND " +
                            "pg2.\"Product\" != pg3.\"Product\" AND " +
                            "pg1.\"Product\" < pg3.\"Product\" AND " +
                            "pg2.\"Product\" < pg3.\"Product\" " +
                        ")" +
                    ")" +

                    "SELECT items, COUNT(*) AS frequency " +
                    "FROM order_pairs " +
                    "GROUP by items " +
                    "ORDER BY frequency DESC " +
                    "LIMIT 10;";

                    command.CommandText = query;
                    command.CommandType = CommandType.Text;

                    context.Database.OpenConnection();

                    using (var result = command.ExecuteReader())
                    {
                        while (result.Read())
                        {
                            results.Add(new KeyValuePair<string, int>(result.GetString("items"), result.GetInt32("frequency")));
                        }
                    }

                    return results;
                }
            }
        }
    }
}