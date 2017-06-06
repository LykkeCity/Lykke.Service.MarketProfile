using System.Collections.Generic;

namespace Lykke.AssetsApi.Core
{
    public class ApplicationSettings
    {
        public AssetsApiSettings AssetsApi { get; set; }

        public IDictionary<string, string> CandleHistoryAssetConnections { get; set; }

        public class AssetsApiSettings
        {
            public DbSettings Db { get; set; }
        }

        public class DbSettings
        {
            public string HTradesConnString { get; set; }
            public string BalancesInfoConnString { get; set; }
            public string HLiquidityConnString { get; set; }
            public string DictsConnString { get; set; }
        }
    }
}