using DbSafe;
using System;
using System.Configuration;

namespace SqlDbSafe
{
    public class SqlDbSafeManager : DbSafeManager<SqlDatabaseClient>
    {
        private readonly SqlDatabaseClient _databaseClient = new SqlDatabaseClient();

        public static SqlDbSafeManager Initialize(DbSafeManagerConfig config, params string[] filenames)
        {
            var result = new SqlDbSafeManager(config);
            foreach (var filename in filenames)
            {
                result.Load(filename);
            }

            result.BeginTest();
            return result;
        }

        public static SqlDbSafeManager Initialize(params string[] filenames)
        {
            return Initialize(DbSafeManagerConfig.GlobalConfig, filenames);
        }

        private SqlDbSafeManager(DbSafeManagerConfig config)
            : base(config)
        {
            DatabaseClient = _databaseClient;
        }

        public SqlDbSafeManager SetConnectionString(string connectionStringName)
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
