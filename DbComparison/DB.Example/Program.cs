using Cassandra.Repository;
using DB.SharedUtils;

namespace DB.Example
{
    internal class Program
    {
        private const int TOTAL_TRANSACTIONS = 10000;

        private static DateTime FROM = new DateTime(2017, 01, 01);
        private static DateTime TILL = new DateTime(2018, 06, 30);

        static async Task Main(string[] args)
        {
            RunCassandra();

            RunPostgres();
        }

        private static void RunCassandra()
        {
            Console.WriteLine("######################################################################");
            Console.WriteLine($"### Cassandra example: {TOTAL_TRANSACTIONS} transactions data set. ###");
            Console.WriteLine("######################################################################");

            var watch = new Watch();
            var repo = new DataProvider();

            //Console.WriteLine("Dropping old table...");
            //repo.DropTable();

            //Console.WriteLine("Creating new table...");
            //repo.CreateTable();

            //watch.Start();
            //Console.WriteLine($"Seeding data of {TOTAL_TRANSACTIONS} transactions...");
            //repo.SeedData(TOTAL_TRANSACTIONS);
            //watch.Stop();

            var totalRows = repo.GetTotalRowCount();
            Console.WriteLine($"Data set contains {totalRows} records...");

            //watch.Start();
            //Console.WriteLine("Reading data range...");
            //var records = repo.SelectRange(FROM, TILL);
            //Console.WriteLine($"Selected {records.Count()} records.");
            //watch.Stop();
            //records.Print();

            watch.Start();
            Console.Write("   1. Calculating total quantity: ");
            var result = repo.GetTotalQuantity();
            Console.WriteLine(result);
            watch.Stop();

            watch.Start();
            Console.Write("   2. Calculating total price: ");
            result = repo.GetTotalPrice();
            Console.WriteLine(result);
            watch.Stop();

            watch.Start();
            Console.Write($"   3. Calculating total price for period {FROM.ToShortDateString()} - {TILL.ToShortDateString()}: ");
            result = repo.GetTotalPrice(FROM, TILL);
            Console.WriteLine(result);
            watch.Stop();

            watch.Start();
            var store = "A";
            var product = "bread";
            Console.Write($"   4. Calculating total quantity of {product} sold in {store} for period {FROM.ToShortDateString()} - {TILL.ToShortDateString()}: ");
            result = repo.GetTotalQuantity(store, product, FROM, TILL);
            Console.WriteLine(result);
            watch.Stop();

            watch.Start();
            product = "bread";
            Console.Write($"   5. Calculating total quantity of {product} sold in all stores for period {FROM.ToShortDateString()} - {TILL.ToShortDateString()}: ");
            result = repo.GetTotalQuantity(product, FROM, TILL);
            Console.WriteLine(result);
            watch.Stop();

            watch.Start();
            Console.WriteLine($"   6. Calculating total price by stores for period {FROM.ToShortDateString()} - {TILL.ToShortDateString()}: ");
            var aggregatedPrices = repo.GetTotalPriceByStores(FROM, TILL);
            foreach (var resultStore in aggregatedPrices.Keys)
            {
                Console.WriteLine($"     - Store {resultStore}: {aggregatedPrices[resultStore]};");
            }
            watch.Stop();

            watch.Start();
            Console.WriteLine($"   7. Calculating products mostly purchased by 2 for period {FROM.ToShortDateString()} - {TILL.ToShortDateString()}: ");
            var top10 = repo.GetProductsPurchasedBy2(FROM, TILL);
            foreach (var pair in top10)
            {
                Console.WriteLine($"    - {pair.Key}: {pair.Value}.");
            }
            watch.Stop();

            watch.Start();
            Console.WriteLine($"   8. Calculating products mostly purchased by 3 for period {FROM.ToShortDateString()} - {TILL.ToShortDateString()}: ");
            top10 = repo.GetProductsPurchasedBy3(FROM, TILL);
            foreach (var pair in top10)
            {
                Console.WriteLine($"    - {pair.Key}: {pair.Value}.");
            }
            watch.Stop();

            watch.Start();
            Console.WriteLine($"   9. Calculating products mostly purchased by 4 for period {FROM.ToShortDateString()} - {TILL.ToShortDateString()}: ");
            top10 = repo.GetProductsPurchasedBy4(FROM, TILL);
            foreach (var pair in top10)
            {
                Console.WriteLine($"    - {pair.Key}: {pair.Value}.");
            }
            watch.Stop();
        }

        private static void RunPostgres()
        {
            Console.WriteLine("#####################################################################");
            Console.WriteLine($"### Postgres example: {TOTAL_TRANSACTIONS} transactions data set. ###");
            Console.WriteLine("#####################################################################");

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            var watch = new Watch();
            var repo = new Pg.Repository.DataProvider();

            repo.Init();

            watch.Start();
            Console.WriteLine("Deleting all records...");
            repo.DeleteAll();
            Console.WriteLine("Done");
            watch.Stop();

            watch.Start();
            Console.WriteLine($"Seeding data of {TOTAL_TRANSACTIONS} transactions...");
            repo.SeedData(TOTAL_TRANSACTIONS);
            watch.Stop();

            watch.Start();
            Console.WriteLine("Reading data range...");
            var records = repo.SelectRange(FROM, TILL);
            Console.WriteLine($"Selected {records.Count()} records.");
            watch.Stop();
            //records.Print();
        }
    }
}