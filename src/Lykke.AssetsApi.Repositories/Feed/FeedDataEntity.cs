using System;
using Lykke.AssetsApi.Core.Domain.Feed;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AssetsApi.Repositories.Feed
{
    public class FeedDataEntity : TableEntity, IFeedData
    {
        public static class Profile
        {
            public static string GeneratePartitionKey()
            {
                return "Feed";
            }

            public static string GenerateRowKey(string asset)
            {
                return asset;
            }

            public static FeedDataEntity CreateNew(IFeedData feedData)
            {
                return new FeedDataEntity
                {
                    PartitionKey = GeneratePartitionKey(),
                    RowKey = GenerateRowKey(feedData.Asset),
                    DateTime = feedData.DateTime,
                    Bid = feedData.Bid,
                    Ask = feedData.Ask
                };
            }

        }


        public string Asset => RowKey;
        public DateTime DateTime { get; set; }
        public double Bid { get; set; }
        public double Ask { get; set; }



    }
}