using System.Data;
using System.Data.CData.ApacheHBase;

namespace HBase.Repository
{
    public class CDataExample
    {
        private const string CONNECTION_STRING = $"Server={Config.HOST};Port={Config.PORT};";

        public void Connect()
        {
            using (var connection = new ApacheHBaseConnection(CONNECTION_STRING))
            {
                connection.Open();

                connection.Close();
            }
        }

        public void Read()
        {
            using (var connection = new ApacheHBaseConnection(CONNECTION_STRING))
            {
                var command = new ApacheHBaseCommand("SELECT * FROM personal", connection);

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Console.WriteLine(String.Format("\t{0} --> \t\t{1}", reader["personal_data:name"], reader["personal_data:city"]));
                }
            }
        }

        public void ReadAccount()
        {
            using (var connection = new ApacheHBaseConnection(CONNECTION_STRING))
            {
                var command = new ApacheHBaseCommand("SELECT * FROM Account", connection);

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Console.WriteLine(String.Format("\t{0} --> \t{1}", reader["account_data:id"], reader["account_data:Name"]));
                }
            }
        }

        public void Insert(string[] values)
        {
            var adapter = new ApacheHBaseDataAdapter();

            using (var conn = new ApacheHBaseConnection(CONNECTION_STRING))
            {
                conn.Open();

                adapter.InsertCommand = conn.CreateCommand();
                adapter.InsertCommand.CommandText = "INSERT INTO Account (RowKey, Name) VALUES (@RowKey, @Name)";
                //adapter.InsertCommand.CommandText = "put 'Account', @RowKey, 'account_data:Name', @Name";
                adapter.InsertCommand.UpdatedRowSource = UpdateRowSource.None;
                adapter.InsertCommand.Parameters.Add(new ApacheHBaseParameter("@RowKey", DbType.Int32, "RowKey"));
                adapter.InsertCommand.Parameters.Add(new ApacheHBaseParameter("@Name", DbType.String, "Name"));

                DataTable batchDataTable = new DataTable();
                batchDataTable.Columns.Add("RowKey", typeof(string));
                batchDataTable.Columns.Add("Name", typeof(string));

                for (int i = 0; i < values.Length; i++)
                {
                    batchDataTable.Rows.Add(i+1, values[i]);
                }

                adapter.UpdateBatchSize = values.Length;
                adapter.Update(batchDataTable);
            }
        }
    }
}
