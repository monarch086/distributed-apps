using Cassandra.Repository;
using DB.SharedUtils;
using static DB.SharedUtils.Display;

namespace HBase.Example
{
    internal class Program
    {
        private const int TOTAL_RECORDS = 10000;

        private static DateTime FROM = new DateTime(2017, 01, 01);
        private static DateTime TILL = new DateTime(2017, 12, 31);

        static async Task Main(string[] args)
        {
            RunCassandra();
        }

        private static void RunCassandra()
        {
            Console.WriteLine($"Cassandra example: {TOTAL_RECORDS} records data set.");

            var watch = new Watch();
            var repo = new DataProvider();

            //Console.WriteLine("Dropping old table...");
            //repo.DropTable();

            //Console.WriteLine("Creating new table...");
            //repo.CreateTable();

            //watch.Start();
            //Console.WriteLine($"Seeding data of {TOTAL_RECORDS} records...");
            //repo.SeedData(TOTAL_RECORDS);
            //watch.Stop();

            watch.Start();
            Console.WriteLine("Reading data range...");
            var records = repo.SelectRange(FROM, TILL);
            Console.WriteLine($"Selected {records.Count()} records.");
            watch.Stop();
            records.Print();

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
        }
    }
}