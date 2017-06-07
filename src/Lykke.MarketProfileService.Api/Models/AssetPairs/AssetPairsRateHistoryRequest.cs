using System;

namespace Lykke.MarketProfileService.Api.Models.AssetPairs
{
    public class AssetPairsRateHistoryRequest
    {
        public string[] AssetPairIds { get; set; }
        public Period Period { get; set; }
        public DateTime DateTime { get; set; }
    }
}