using System;
using Lykke.Domain.Prices.Contracts;

namespace Lykke.MarketProfileService.Core.Domain
{
    public class AssetPair : IAssetPair
    {
        public string Code { get; set; }
        public double BidPrice { get; set; }
        public double AskPrice { get; set; }
        public DateTime BidPriceTimestamp { get; set; }
        public DateTime AskPriceTimestamp { get; set; }

        public IAssetPair ProcessQuote(IQuote quote)
        {
            if (quote.IsBuy)
            {
                if (BidPriceTimestamp < quote.Timestamp)
                {
                    BidPrice = quote.Price;
                    BidPriceTimestamp = quote.Timestamp;
                }
            }
            else
            {
                if (AskPriceTimestamp < quote.Timestamp)
                {
                    AskPrice = quote.Price;
                    AskPriceTimestamp = quote.Timestamp;
                }
            }

            return this;
        }

        public static AssetPair Create(IQuote quote)
        {
            var pair = new AssetPair
            {
                Code = quote.AssetPair,
                AskPriceTimestamp = DateTime.MinValue,
                BidPriceTimestamp = DateTime.MinValue
            };

            pair.ProcessQuote(quote);

            return pair;
        }

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