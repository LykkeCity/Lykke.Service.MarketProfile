using JetBrains.Annotations;
using Lykke.Sdk.Settings;

namespace Lykke.Service.MarketProfile.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public MarketProfileServiceSettings MarketProfileService { get; set; }

        public class MarketProfileServiceSettings
        {
            public DbSettings Db { get; set; }
            public RedisSettings Redis { get; set; }
        }

        public class DbSettings
        {
            public string LogsConnectionString { get; set; }
        }

        public class RedisSettings
        {
            public string Configuration { get; set; }
        }
    }
}
