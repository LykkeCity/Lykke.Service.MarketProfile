using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.MarketProfileService.Core.Domain.Feed;

namespace Lykke.MarketProfileService.Repositories.Feed
{
    public class FeedHistoryRepository : IFeedHistoryRepository
    {
        private readonly INoSQLTableStorage<FeedHistoryEntity> _tableStorage;

        public FeedHistoryRepository(INoSQLTableStorage<FeedHistoryEntity> tableStorage)
        {
            _tableStorage = tableStorage;
        }

        public async Task<IFeedHistory> GetAsync(string assetPairId, string priceType, DateTime feedTime)
        {
            var entity = await _tableStorage.GetDataAsync(FeedHistoryEntity.GeneratePartition(assetPairId, priceType),
                FeedHistoryEntity.GenerateRowKey(feedTime));
            return entity?.ToDto();
        }

        public async Task<IEnumerable<IFeedHistory>> GetAsync(string assetPairId, string priceType,
            DateTime from, DateTime to)
        {
            var entities = await _tableStorage.GetDataAsync(FeedHistoryEntity.GeneratePartition(assetPairId, priceType),
                entity =>
                {
                    var dt = FeedHistoryExt.ParseFeedTime(entity.RowKey);
                    return dt > from && dt < to;
                });
            return entities.Select(x => x.ToDto());
        }

        public async Task<IEnumerable<IFeedHistory>> GetLastTenMinutesAskAsync(string assetPairId)
        {
            var rangeQuery = AzureStorageUtils.QueryGenerator<FeedHistoryEntity>
                .GreaterThanQuery(FeedHistoryEntity.GeneratePartition(assetPairId, PriceType.Ask),
                    FeedHistoryEntity.GenerateRowKey(DateTime.UtcNow.AddMinutes(-10)));

            return (await _tableStorage.WhereAsync(rangeQuery)).Select(x => x.ToDto());
        }

        public async Task<IFeedHistory> GetСlosestAvailableAsync(string assetPairId, string priceType, DateTime feedTime)
        {
            var rangeQuery = AzureStorageUtils.QueryGenerator<FeedHistoryEntity>
                .GreaterThanQuery(FeedHistoryEntity.GeneratePartition(assetPairId, priceType),
                    FeedHistoryEntity.GenerateRowKey(feedTime)).Take(1);

            var resList = new List<FeedHistoryEntity>();
            await _tableStorage.ExecuteAsync(rangeQuery, entities =>
            {
                resList.AddRange(entities);
            }, () => false);

            return resList.Any() ? resList.First().ToDto() : null;
        }
    }
}