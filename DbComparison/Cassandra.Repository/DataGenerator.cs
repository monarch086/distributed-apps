using DataModel;

namespace Cassandra.Repository
{
    internal class DataGenerator
    {
        private Random Rnd => new Random();
        private string[] stores = new string[] { "A", "B", "C", "D" };
        private string[] products = new string[] { "bread", "milk", "eggs", "meat" };
        private int[] months = Enumerable.Range(1, 12).ToArray();
        private int[] days = Enumerable.Range(1, 28).ToArray();
        private int[] hours = Enumerable.Range(0, 23).ToArray();

        public Record GenerateRecord(int id, DateTime? date = null)
        {
            return new Record
            {
                Id = id,
                Date = date ?? GetRandomDate(2022),
                Store = GetRandomArrayElement(stores),
                Product = GetRandomArrayElement(products),
                Price = GetRandomDecimal(200),
                Quantity = GetRandomDecimal(10)
            };
        }

        public DateTime[] GenerateDates(int total, int fromYear, int tillYear)
        {
            var yearsCount = tillYear - fromYear;
            var years = Enumerable.Range(fromYear, yearsCount).ToArray();

            return Enumerable.Range(1, total)
                .Select(_ => GetRandomDate(GetRandomArrayElement(years)))
                .OrderBy(x => x)
                .ToArray();
        }

        private DateTime GetRandomDate(int year)
        {
            int month = GetRandomArrayElement(months);
            int day = GetRandomArrayElement(days);
            int hour = GetRandomArrayElement(hours);
            return new DateTime(year, month, day, hour, 0, 0);
        }

        private T GetRandomArrayElement<T>(T[] array)
        {
            return array[Rnd.Next(array.Length)];
        }

        private decimal GetRandomDecimal(int max)
        {
            var num = Rnd.Next(max);
            var fraction = Rnd.NextDouble();

            return Math.Round(new decimal(num + fraction), 2);
        }
    }
}
