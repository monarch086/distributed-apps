using DB.SharedUtils;

namespace DB.Example
{
    internal class Postgres
    {
        public static void Run(int transactionsCount, DateTime from, DateTime till)
        {
            Console.WriteLine("#####################################################################");
            Console.WriteLine($"### Postgres example: {transactionsCount} transactions data set. ###");
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
            Console.WriteLine($"Seeding data of {transactionsCount} transactions...");
            repo.SeedData(transactionsCount);
            watch.Stop();

            watch.Start();
            Console.WriteLine("Reading data range...");
            var records = repo.SelectRange(from, till);
            Console.WriteLine($"Selected {records.Count()} records.");
            watch.Stop();
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
            Console.Write($"   3. Calculating total price for period {from.ToShortDateString()} - {till.ToShortDateString()}: ");
            result = repo.GetTotalPrice(from, till);
            Console.WriteLine(result);
            watch.Stop();

            watch.Start();
            var store = "A";
            var product = "bread";
            Console.Write($"   4. Calculating total quantity of {product} sold in {store} for period {from.ToShortDateString()} - {till.ToShortDateString()}: ");
            result = repo.GetTotalQuantity(store, product, from, till);
            Console.WriteLine(result);
            watch.Stop();

            watch.Start();
            product = "bread";
            Console.Write($"   5. Calculating total quantity of {product} sold in all stores for period {from.ToShortDateString()} - {till.ToShortDateString()}: ");
            result = repo.GetTotalQuantity(product, from, till);
            Console.WriteLine(result);
            watch.Stop();
        }
    }
}
