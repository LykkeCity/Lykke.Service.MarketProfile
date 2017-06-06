using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AssetsApi.Repositories.Feed
{
    public class FeedHistoryEntity : TableEntity
    {
        public string Data { get; set; }

        public static string GeneratePartition(string assetId, string priceType)
        {
            return $"{assetId}_{priceType}";
        }

        public static string GenerateRowKey(DateTime feedTime)
        {
            return $"{feedTime.Year}{feedTime.Month:00}{feedTime.Day:00}{feedTime.Hour:00}{feedTime.Minute:00}";
        }
    }
}