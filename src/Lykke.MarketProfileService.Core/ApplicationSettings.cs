using System;

namespace Lykke.MarketProfileService.Core
{
    public class ApplicationSettings
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
            public string ConnectionString { get; set; }
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
    }
}