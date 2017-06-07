using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Lykke.MarketProfileService.Core.Domain.Feed;

namespace Lykke.MarketProfileService.Repositories.Feed
{
    public static class FeedHistoryExt
    {
        public static FeedHistoryDto ToDto(this FeedHistoryEntity entity)
        {
            var dto = new FeedHistoryDto();

            //example: "BTCCHF_Bid"
            var assetPriceTypeVals = entity.PartitionKey.Split('_');
            dto.AssetPair = assetPriceTypeVals[0];
            dto.PriceType = assetPriceTypeVals[1];

            dto.TradeCandles = ParseCandles(entity.Data);
            dto.FeedTime = ParseFeedTime(entity.RowKey);

            return dto;
        }

        public static DateTime ParseFeedTime(string rowKey)
        {
            //example: 201604290745
            int year = int.Parse(rowKey.Substring(0, 4));
            int month = int.Parse(rowKey.Substring(4, 2));
            int day = int.Parse(rowKey.Substring(6, 2));
            int hour = int.Parse(rowKey.Substring(8, 2));
            int min = int.Parse(rowKey.Substring(10, 2));
            return new DateTime(year, month, day, hour, min, 0);
        }

        private static TradeCandle[] ParseCandles(string data)
        {
            var candlesList = new List<TradeCandle>();
            if (!string.IsNullOrEmpty(data))
            {
                var candles = data.Split('|');
                foreach (var candle in candles)
                {
                    if (!string.IsNullOrEmpty(candle))
                    {
                        //parameters example: O=446.322;C=446.322;H=446.322;L=446.322;T=30
                        var parameters = candle.Split(';');

                        var tradeCandle = new TradeCandle();
                        foreach (var nameValuePair in parameters.Select(parameter => parameter.Split('=')))
                        {
                            switch (nameValuePair[0])
                            {
                                case "O":
                                    tradeCandle.Open = nameValuePair[1].ParseAnyDouble();
                                    break;
                                case "C":
                                    tradeCandle.Close = nameValuePair[1].ParseAnyDouble();
                                    break;
                                case "H":
                                    tradeCandle.High = nameValuePair[1].ParseAnyDouble();
                                    break;
                                case "L":
                                    tradeCandle.Low = nameValuePair[1].ParseAnyDouble();
                                    break;
                                case "T":
                                    tradeCandle.Seconds = int.Parse(nameValuePair[1]);
                                    break;
                                default:
                                    throw new ArgumentException("unexpected value");
                            }
                        }
                        candlesList.Add(tradeCandle);
                    }
                }
            }

            return candlesList.ToArray();
        }
    }
}