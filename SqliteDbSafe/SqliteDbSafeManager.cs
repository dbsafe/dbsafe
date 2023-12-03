using DbSafe;
using Microsoft.Data.Sqlite;

namespace SqliteDbSafe
{
    public class SqliteDbSafeManager : AdoDbSafeManager<SqliteDatabaseClient, SqliteConnection, SqliteCommand>
    {
        public static SqliteDbSafeManager Initialize(DbSafeManagerConfig config, params string[] filenames)
        {
            var result = new SqliteDbSafeManager(config);
            foreach (var filename in filenames)
            {
                result.Load(filename);
            }

            result.BeginTest();
            return result;
        }

        public static SqliteDbSafeManager Initialize(params string[] filenames)
        {
            return Initialize(DbSafeManagerConfig.GlobalConfig, filenames);
        }

        private SqliteDbSafeManager(DbSafeManagerConfig config)
            : base(config, new SqliteDatabaseClient(config.ReuseConnection))
        {
        }
    }
}