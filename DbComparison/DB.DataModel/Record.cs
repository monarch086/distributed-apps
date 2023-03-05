namespace DataModel
{
    public class Record
    {
        public int Id { get; set; }

        public int TransactionId { get; set; }

        public DateTime Date { get; set; }

        public string Store { get; set; }

        public string Product { get; set; }

        public decimal Price { get; set; }

        public decimal Quantity { get; set; }
    }
}
