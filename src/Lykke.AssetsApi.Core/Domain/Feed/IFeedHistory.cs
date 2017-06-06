using System;

namespace Lykke.AssetsApi.Core.Domain.Feed
{
    public interface IFeedHistory
    {
        string AssetPair { get; }
        string PriceType { get; }
        DateTime FeedTime { get; }
        TradeCandle[] TradeCandles { get; }
    }
}