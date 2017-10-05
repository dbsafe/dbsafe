namespace DbSafe
{
    public class DbSafeManagerConfig
    {
        public static DbSafeManagerConfig GlobalConfig { get; } = new DbSafeManagerConfig { IsGlobalConfig = true };

        public bool SerializeTests { get; set; } = true;
        public bool IsGlobalConfig { get; private set; } = false;
    }
}
