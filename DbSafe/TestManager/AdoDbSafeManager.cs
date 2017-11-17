using System;
using System.Configuration;
using System.Data.Common;

namespace DbSafe
{
    public abstract class AdoDbSafeManager<TAdoDatabaseClient, TDbConnection, TDbCommand> : DbSafeManager<TAdoDatabaseClient>
            where TAdoDatabaseClient : AdoDatabaseClient<TDbConnection, TDbCommand>, IDatabaseClient
            where TDbConnection : DbConnection
            where TDbCommand : DbCommand
    {
        protected readonly TAdoDatabaseClient _databaseClient;

        public AdoDbSafeManager(DbSafeManagerConfig config, TAdoDatabaseClient databaseClient)
            : base(config)
        {
            _databaseClient = databaseClient;
            DatabaseClient = _databaseClient;
        }

        public AdoDbSafeManager<TAdoDatabaseClient, TDbConnection, TDbCommand> SetConnectionString(string connectionStringName)
        {
            var connectionStringDetail = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (connectionStringDetail == null)
            {
                string message = $"Connection String '{connectionStringName}' not found";
                throw new Exception(message);
            }

            _databaseClient.ConnectionString = connectionStringDetail.ConnectionString;
            return this;
        }

        public AdoDbSafeManager<TAdoDatabaseClient, TDbConnection, TDbCommand> PassConnectionString(string connectionString)
        {
            _databaseClient.ConnectionString = connectionString;
            return this;
        }

        protected override void ValidateDependencies(bool allowTestWithoutInputFile = false)
        {
            base.ValidateDependencies(allowTestWithoutInputFile);
            if (string.IsNullOrWhiteSpace(_databaseClient.ConnectionString))
            {
                throw new InvalidOperationException("ConnectionString not specified");
            }
        }
    }
}
