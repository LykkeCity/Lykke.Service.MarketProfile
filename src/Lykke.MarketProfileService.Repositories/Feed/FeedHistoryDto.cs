using System;
using Lykke.MarketProfileService.Core.Domain.Feed;

namespace Lykke.MarketProfileService.Repositories.Feed
{
    public class FeedHistoryDto : IFeedHistory
    {
        public string AssetPair { get; set; }
        public string PriceType { get; set; }
        public DateTime FeedTime { get; set; }
        public TradeCandle[] TradeCandles { get; set; }
    }
}