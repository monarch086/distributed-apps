using Cassandra.Mapping;
using DataModel;
using DB.SharedUtils;

namespace Cassandra.Repository
{
    public class DataProvider
    {
        private const string HOST = "127.0.0.1";
        private const string KEY_SPACE = "tutorial";

        private const string TABLE_NAME = "tutorial.products";
        private readonly DataGenerator DataGenerator = new DataGenerator();

        public void CreateTable()
        {
            using (var session = Connect())
            {
                var query = @$"
                CREATE TABLE IF NOT EXISTS {TABLE_NAME} (
                    id int,
                    transactionid int,
                    date timestamp,
                    store text,
                    product text,
                    price decimal,
                    quantity decimal,
                    PRIMARY KEY(id));";
                session.Execute(query);

                query = $"CREATE INDEX ON {TABLE_NAME}(date);";
                session.Execute(query);
            }
        }

        public void DropTable()
        {
            using (var session = Connect())
            {
                var query = @$"DROP TABLE IF EXISTS {TABLE_NAME};";
                session.Execute(query);
            }
        }

        public void CheckConnection()
        {
            using (var session = Connect())
            {
                var rs = session.Execute("SELECT * FROM tutorial.posts");

                foreach (var row in rs)
                {
                    var id = row.GetValue<int>("id");
                    var content = row.GetValue<string>("content");
                    var title = row.GetValue<string>("title");

                    Console.WriteLine($"{id}: {title} - {content}");
                }
            }
        }

        public void SeedData(int total = 1000)
        {
            using (var session = Connect())
            {
                var query = @$"INSERT INTO {TABLE_NAME} (id, transactionid, date, store, product, price, quantity)
                               VALUES (?, ?, ?, ?, ?, ?, ?);";

                var ps = session.Prepare(query);

                var dates = DataGenerator.GenerateDates(total, 2015, 2022);

                var recordId = 1;

                foreach (var transactionId in Enumerable.Range(1, total))
                {
                    var transactionProductsCount = DataGenerator.GetRandomArrayElement(Enumerable.Range(1, 10).ToArray());
                    var date = dates[transactionId - 1];
                    var store = DataGenerator.GetRandomArrayElement(DataGenerator.STORES);

                    foreach (var product in Enumerable.Range(1, transactionProductsCount))
                    {
                        var record = DataGenerator.GenerateRecord(recordId++, transactionId, store, date);
                        var statement = ps.Bind(record.Id, record.TransactionId, record.Date, record.Store, record.Product, record.Price, record.Quantity);

                        var rs = session.Execute(statement);
                    }

                    if (transactionId % 1000 == 0) Console.WriteLine($"{transactionId * 100 / total}% are done...");
                }
            }
        }

        public int GetTotalRowCount()
        {
            using (var session = Connect())
            {
                var mapper = new Mapper(session);

                var query = @$"SELECT COUNT (*)
                               FROM {TABLE_NAME};";

                return mapper.Single<int>(query);
            }
        }

        public IEnumerable<Record> SelectRange(DateTime from, DateTime till)
        {
            using (var session = Connect())
            {
                var mapper = new Mapper(session);
                var query = @$"SELECT id, date, store, product, price, quantity
                               FROM {TABLE_NAME}
                               WHERE date >= ? AND date <= ?
                               ALLOW FILTERING;";

                var records = mapper.Fetch<Record>(query, from, till);

                return records;
            }
        }

        public decimal GetTotalQuantity()
        {
            using (var session = Connect())
            {
                var mapper = new Mapper(session);
                var query = @$"SELECT SUM (quantity)
                               FROM {TABLE_NAME};";

                return mapper.Single<decimal>(query);
            }
        }

        public decimal GetTotalPrice()
        {
            using (var session = Connect())
            {
                var mapper = new Mapper(session);
                var query = @$"SELECT price, quantity
                               FROM {TABLE_NAME};";

                var rs = session.Execute(query);

                return rs.Aggregate(new decimal(0), (a, b) => a + b.GetValue<decimal>("price") * b.GetValue<decimal>("quantity"));
            }
        }

        public decimal GetTotalPrice(DateTime from, DateTime till)
        {
            using (var session = Connect())
            {
                var mapper = new Mapper(session);
                var query = @$"SELECT price, quantity
                               FROM {TABLE_NAME}
                               WHERE date >= ? AND date <= ?
                               ALLOW FILTERING;";
                var ps = session.Prepare(query);
                var statement = ps.Bind(from, till);
                var rs = session.Execute(statement);

                return rs.Aggregate(new decimal(0), (a, b) => a + b.GetValue<decimal>("price") * b.GetValue<decimal>("quantity"));
            }
        }

        public decimal GetTotalQuantity(string store, string product, DateTime from, DateTime till)
        {
            using (var session = Connect())
            {
                var mapper = new Mapper(session);
                var query = @$"SELECT SUM (quantity)
                               FROM {TABLE_NAME}
                               WHERE date >= ? AND date <= ?
                                AND store = ?
                                AND product = ?
                               ALLOW FILTERING;";

                return mapper.Single<decimal>(query, from, till, store, product);
            }
        }

        public decimal GetTotalQuantity(string product, DateTime from, DateTime till)
        {
            using (var session = Connect())
            {
                var mapper = new Mapper(session);
                var query = @$"SELECT SUM (quantity)
                               FROM {TABLE_NAME}
                               WHERE date >= ? AND date <= ?
                                    AND product = ?
                               ALLOW FILTERING;";

                //var values = mapper.Fetch<decimal>(query, from, till, product);

                //return values.Aggregate((a, b) => a + b);

                return mapper.Single<decimal>(query, from, till, product);
            }
        }

        public IDictionary<string, decimal> GetTotalPriceByStores(DateTime from, DateTime till)
        {
            using (var session = Connect())
            {
                var mapper = new Mapper(session);
                var query = @$"SELECT store, price, quantity
                               FROM {TABLE_NAME}
                               WHERE date >= ? AND date <= ?
                               ALLOW FILTERING;";
                var ps = session.Prepare(query);
                var statement = ps.Bind(from, till);
                var rs = session.Execute(statement);

                var grouped = new Dictionary<string, List<Row>>();

                foreach (var row in rs)
                {
                    var store = row.GetValue<string>("store");

                    if (grouped.ContainsKey(store))
                    {
                        grouped[store].Add(row);
                    }
                    else
                    {
                        grouped.Add(store, new List<Row>());
                    }
                }

                var aggregated = new Dictionary<string, decimal>();

                foreach (var store in grouped.Keys)
                {
                    aggregated.Add(store, grouped[store].Aggregate(new decimal(0), (a, b) => a + b.GetValue<decimal>("price") * b.GetValue<decimal>("quantity")));
                }

                return aggregated;
            }
        }

        public void GetPurchasedTogetherProductsBy2()
        {
            using (var session = Connect())
            {
                var query = @$"";
            }
        }

        private ISession Connect()
        {
            var cluster = Cluster.Builder()
                     .AddContactPoint(HOST)
                     .WithLoadBalancingPolicy(new TokenAwarePolicy(new DCAwareRoundRobinPolicy("datacenter1")))
                     .Build();

            return cluster.Connect(KEY_SPACE);
        }
    }
}