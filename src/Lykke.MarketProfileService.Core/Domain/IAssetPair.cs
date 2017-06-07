using System;

namespace Lykke.MarketProfileService.Core.Domain
{
    public interface IAssetPair
    {
        string Code { get; set; }
        double BidPrice { get; set; }
        double AskPrice { get; set; }
        DateTime BidPriceTimestamp { get; set; }
        DateTime AskPriceTimestamp { get; set; }
    }
}