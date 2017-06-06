using System;

namespace Lykke.AssetsApi.Models.AssetPairs
{
    public class AssetPairsRateHistoryRequest
    {
        public string[] AssetPairIds { get; set; }
        public Period Period { get; set; }
        public DateTime DateTime { get; set; }
    }
}