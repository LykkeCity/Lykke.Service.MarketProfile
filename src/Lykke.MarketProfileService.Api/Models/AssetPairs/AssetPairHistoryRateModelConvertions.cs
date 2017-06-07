using System.Collections.Generic;
using System.Linq;
using Lykke.Domain.Prices.Contracts;
using Lykke.Domain.Prices.Model;
using Lykke.MarketProfileService.Core.Domain.Feed;

namespace Lykke.MarketProfileService.Api.Models.AssetPairs
{
    public static class AssetPairHistoryRateModelConvertions
    {
        public static IEnumerable<AssetPairHistoryRateModel> ToApiModel(this IEnumerable<IFeedHistory> feedHistories)
        {
            var grouped = feedHistories
                .Select(feedHistory =>
                {
                    var lastCandle = feedHistory.TradeCandles.Last();

                    return new
                    {
                        AssetPairId = feedHistory.AssetPair,
                        Candle = new FeedCandle
                        {
                            DateTime = feedHistory.FeedTime,
                            Close = lastCandle.Close,
                            High = lastCandle.High,
                            IsBuy = feedHistory.PriceType == TradePriceType.Bid,
                            Low = lastCandle.Low,
                            Open = lastCandle.Open
                        }
                    };
                })
                .GroupBy(x => x.AssetPairId, x => x.Candle);

            var result = new List<AssetPairHistoryRateModel>();

            foreach (var group in grouped)
            {
                var buyCandle = group.First(x => x.IsBuy);
                var sellCandle = group.First(x => !x.IsBuy);

                result.Add(ToApiModel(group.Key, buyCandle, sellCandle));
            }

            return result;
        }

        public static AssetPairHistoryRateModel ToApiModel(string assetPairId, IFeedCandle buyCandle, IFeedCandle sellCandle)
        {
            return new AssetPairHistoryRateModel
            {
                Id = assetPairId,
                Ask = sellCandle?.Close,
                Bid = buyCandle?.Close
            };
        }
    }
}