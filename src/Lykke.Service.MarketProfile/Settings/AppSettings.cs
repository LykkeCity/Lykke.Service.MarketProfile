using System;
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
            public RabbitSettings QuoteFeedRabbitSettings { get; set; }
            public CacheSettings CacheSettings { get; set; }
        }

        public class DbSettings
        {
            public string CachePersistenceConnectionString { get; set; }
            public string LogsConnectionString { get; set; }
        }

        public class RabbitSettings
        {
            public string ConnectionString { get; set; }
            public string ExchangeName { get; set; }
        }

        public class CacheSettings
        {
            public TimeSpan PersistPeriod { get; set; }
        }

        public class SlackNotificationsSettings
        {
            public AzureQueueSettings AzureQueue { get; set; }

            public int ThrottlingLimitSeconds { get; set; }
        }

        public class AzureQueueSettings
        {
            public string ConnectionString { get; set; }

            public string QueueName { get; set; }
        }
    }
}