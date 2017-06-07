using System;

namespace Lykke.MarketProfileService.Api.Models.AssetPairs
{
    public class AssetPairRateHistoryRequest
    {
        public Period Period { get; set; }
        public DateTime DateTime { get; set; }
    }
}