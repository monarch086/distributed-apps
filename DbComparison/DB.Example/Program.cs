namespace DB.Example
{
    internal class Program
    {
        private const int TOTAL_TRANSACTIONS = 100000;

        private static DateTime FROM = new DateTime(2017, 01, 01);
        private static DateTime TILL = new DateTime(2018, 06, 30);

        static async Task Main(string[] args)
        {
            Cassandra.Run(TOTAL_TRANSACTIONS, FROM, TILL);

            Postgres.Run(TOTAL_TRANSACTIONS, FROM, TILL);
        }
    }
}