using DbSafe.FileDefinition;
using System.Collections.Generic;
using System.Data.Common;

namespace DbSafe
{
    public abstract class AdoDatabaseClient<TDbConnection, TDbCommand> : IDatabaseClient
            where TDbConnection : DbConnection
            where TDbCommand : DbCommand
    {
        private readonly bool _reuseConnection = false;
        private readonly Dictionary<string, TDbConnection> _connections = new Dictionary<string, TDbConnection>();
        private readonly object _lock = new object();

        public string ConnectionString { get; set; }

        public AdoDatabaseClient(bool reuseConnection)
        {
            _reuseConnection = reuseConnection;
        }

        public void ExecuteCommand(string command)
        {
            ExecuteCommand(command, ConnectionString);
        }

        public abstract DatasetElement ReadTable(string command, FormatterManager formatter);

        public abstract void WriteTable(DatasetElement dataset);

        protected void ExecuteCommand(string command, string connectionString)
        {
            var conn = GetDbConnection(connectionString);
            try
            {
                conn.Open();
                using (var comm = CreateDbCommand(command, conn))
                {
                    comm.ExecuteNonQuery();
                }
            }
            finally
            {
                DisposeConnection(conn);
            }
        }

        protected abstract TDbConnection CreateDbConnection(string connectionString);

        protected abstract TDbCommand CreateDbCommand(string command, TDbConnection conn);

        private TDbConnection GetDbConnection(string connectionString)
        {
            if (!_reuseConnection)
            {
                return CreateDbConnection(connectionString);
            }

            TDbConnection conn;
            lock (_lock)
            {
                var connectionExists = _connections.ContainsKey(connectionString);
                if (connectionExists)
                {
                    conn = _connections[connectionString];
                }
                else
                {
                    conn = CreateDbConnection(connectionString);
                    _connections[connectionString] = conn;
                }
            }

            return conn;
        }

        private void DisposeConnection(TDbConnection conn)
        {
            if (!_reuseConnection)
            {
                conn.Dispose();
            }
        }
    }
}
