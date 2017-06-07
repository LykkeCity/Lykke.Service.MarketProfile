using System;
using System.Globalization;
using Lykke.MarketProfileService.Core.Domain;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.MarketProfileService.Repositories.Entities
{
    public class AssetPairEntity :
        TableEntity,
        IAssetPair
    {
        public string Code { get; set; }
        public double BidPrice { get; set; }
        public double AskPrice { get; set; }
        public DateTime BidPriceTimestamp { get; set; }
        public DateTime AskPriceTimestamp { get; set; }

        public static string GeneratePartitionKey(IAssetPair pair)
        {
            return nameof(AssetPairEntity);
        }

        public static string GenerateRowKey(IAssetPair pair)
        {
            return pair.Code;
        }

        public static AssetPairEntity Create(IAssetPair pair)
        {
            return new AssetPairEntity
            {
                PartitionKey = GeneratePartitionKey(pair),
                RowKey = GenerateRowKey(pair),
                Code = pair.Code,
                BidPrice = pair.BidPrice,
                AskPrice = pair.AskPrice,
                BidPriceTimestamp = pair.BidPriceTimestamp,
                AskPriceTimestamp = pair.AskPriceTimestamp
            };
        }
    }
}