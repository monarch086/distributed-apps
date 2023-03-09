using Cassandra.Repository;
using DB.SharedUtils;

namespace DB.Example
{
    internal class Cassandra
    {
        public static void Run(int transactionsCount, DateTime from, DateTime till)
        {
            Console.WriteLine("######################################################################");
            Console.WriteLine($"### Cassandra example: {transactionsCount} transactions data set. ###");
            Console.WriteLine("######################################################################");

            var watch = new Watch();
            var repo = new DataProvider();

            //Console.WriteLine("Dropping old table...");
            //repo.DropTable();

            //Console.WriteLine("Creating new table...");
            //repo.CreateTable();

            //watch.Start();
            //Console.WriteLine($"Seeding data of {transactionsCount} transactions...");
            //repo.SeedData(transactionsCount);
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

            watch.Start();
            Console.WriteLine($"   6. Calculating total price by stores for period {from.ToShortDateString()} - {till.ToShortDateString()}: ");
            var aggregatedPrices = repo.GetTotalPriceByStores(from, till);
            foreach (var resultStore in aggregatedPrices.Keys)
            {
                Console.WriteLine($"     - Store {resultStore}: {aggregatedPrices[resultStore]};");
            }
            watch.Stop();

            watch.Start();
            Console.WriteLine($"   7. Calculating products mostly purchased by 2 for period {from.ToShortDateString()} - {till.ToShortDateString()}: ");
            var top10 = repo.GetProductsPurchasedBy2(from, till);
            foreach (var pair in top10)
            {
                Console.WriteLine($"    - {pair.Key}: {pair.Value}.");
            }
            watch.Stop();

            watch.Start();
            Console.WriteLine($"   8. Calculating products mostly purchased by 3 for period {from.ToShortDateString()} - {till.ToShortDateString()}: ");
            top10 = repo.GetProductsPurchasedBy3(from, till);
            foreach (var pair in top10)
            {
                Console.WriteLine($"    - {pair.Key}: {pair.Value}.");
            }
            watch.Stop();

            watch.Start();
            Console.WriteLine($"   9. Calculating products mostly purchased by 4 for period {from.ToShortDateString()} - {till.ToShortDateString()}: ");
            top10 = repo.GetProductsPurchasedBy4(from, till);
            foreach (var pair in top10)
            {
                Console.WriteLine($"    - {pair.Key}: {pair.Value}.");
            }
            watch.Stop();
        }
    }
}
