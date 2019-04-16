using DbSafe;
using Npgsql;

namespace PgDbSafe
{
    public class PgDbSafeManager : AdoDbSafeManager<PgDatabaseClient, NpgsqlConnection, NpgsqlCommand>
    {
        public static PgDbSafeManager Initialize(DbSafeManagerConfig config, params string[] filenames)
        {
            var result = new PgDbSafeManager(config);
            foreach (var filename in filenames)
            {
                result.Load(filename);
            }

            result.BeginTest();
            return result;
        }

        public static PgDbSafeManager Initialize(params string[] filenames)
        {
            return Initialize(DbSafeManagerConfig.GlobalConfig, filenames);
        }

        private PgDbSafeManager(DbSafeManagerConfig config)
            : base(config, new PgDatabaseClient(config.ReuseConnection))
        {
        }
    }
}
