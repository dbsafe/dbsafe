using DbSafe;
using System.Data.SqlClient;

namespace SqlDbSafe
{
    public class SqlDbSafeManager : AdoDbSafeManager<SqlDatabaseClient, SqlConnection, SqlCommand>
    {
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
            : base(config, new SqlDatabaseClient())
        {
        }
    }
}
