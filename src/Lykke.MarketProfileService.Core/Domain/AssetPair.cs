using System;

namespace Lykke.MarketProfileService.Core.Domain
{
    public class AssetPair : IAssetPair
    {
        public string Code { get; set; }
        public double BidPrice { get; set; }
        public double AskPrice { get; set; }
        public DateTime BidPriceTimestamp { get; set; }
        public DateTime AskPriceTimestamp { get; set; }

        public static AssetPair Create(IAssetPair pair)
        {
            return new AssetPair
            {
                Code = pair.Code,
                BidPrice = pair.BidPrice,
                AskPrice = pair.AskPrice,
                BidPriceTimestamp = pair.BidPriceTimestamp,
                AskPriceTimestamp = pair.AskPriceTimestamp
            };
        }
    }
}