using System;

namespace Lykke.AssetsApi.Models.AssetPairs
{
    public class AssetPairRateHistoryRequest
    {
        public Period Period { get; set; }
        public DateTime DateTime { get; set; }
    }
}