using System;

namespace Lykke.MarketProfileService.Core.Domain.Feed
{
    public interface IFeedHistory
    {
        string AssetPair { get; }
        string PriceType { get; }
        DateTime FeedTime { get; }
        TradeCandle[] TradeCandles { get; }
    }
}