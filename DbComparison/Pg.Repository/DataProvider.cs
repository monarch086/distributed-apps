using DataModel;
using DB.SharedUtils;
using Microsoft.EntityFrameworkCore;

namespace Pg.Repository
{
    public class DataProvider
    {
        //private ProductsDbContext _context;
        private readonly DataGenerator _dataGenerator = new DataGenerator();

        public DataProvider()
        {
            //_context = new ProductsDbContext();
        }

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
    }
}