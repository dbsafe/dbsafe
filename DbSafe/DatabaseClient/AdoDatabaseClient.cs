using DbSafe.FileDefinition;
using System.Data.Common;

namespace DbSafe
{
    public abstract class AdoDatabaseClient<TDbConnection, TDbCommand> : IDatabaseClient
            where TDbConnection : DbConnection
            where TDbCommand : DbCommand
    {
        public string ConnectionString { get; set; }

        public void ExecuteCommand(string command)
        {
            ExecuteCommand(command, ConnectionString);
        }

        public abstract DatasetElement ReadTable(string command, FormatterManager formatter);

        public abstract void WriteTable(DatasetElement dataset);

        protected void ExecuteCommand(string command, string connectionString)
        {
            using (var conn = CreateDbConnection(connectionString))
            {
                conn.Open();
                using (var comm = CreateDbCommand(command, conn))
                {
                    comm.ExecuteNonQuery();
                }
            }
        }

        protected abstract TDbConnection CreateDbConnection(string connectionString);

        protected abstract TDbCommand CreateDbCommand(string command, TDbConnection conn);
    }
}
