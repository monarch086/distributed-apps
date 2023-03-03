using DataModel;

namespace DB.SharedUtils
{
    public static class Display
    {
        public static void Print(this IEnumerable<Record> records)
        {
            foreach(var r in records)
            {
                Console.WriteLine($"{r.Id}: {r.Date}, {r.Product}, {r.Store}, {r.Price} - {r.Quantity}");
            }
        }
    }
}