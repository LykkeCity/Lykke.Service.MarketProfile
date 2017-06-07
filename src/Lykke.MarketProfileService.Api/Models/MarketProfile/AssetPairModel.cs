using System;

namespace Lykke.MarketProfileService.Api.Models.MarketProfile
{
    public class AssetPairModel
    {
        public string AssetPair { get; set; }
        public double BidPrice { get; set; }
        public double AskPrice { get; set; }
        public DateTime BidPriceTimestamp { get; set; }
        public DateTime AskPriceTimestamp { get; set; }
    }
}