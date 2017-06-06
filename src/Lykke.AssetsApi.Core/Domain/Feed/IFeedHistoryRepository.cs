using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.AssetsApi.Core.Domain.Feed
{
    public interface IFeedHistoryRepository
    {
        Task<IFeedHistory> GetAsync(string assetPairId, string priceType, DateTime feedTime);
        Task<IEnumerable<IFeedHistory>> GetAsync(string assetPairId, string priceType, DateTime from, DateTime to);
        Task<IEnumerable<IFeedHistory>> GetLastTenMinutesAskAsync(string assetPairId);

        // temporary due to missed data in FeedHistory
        Task<IFeedHistory> GetСlosestAvailableAsync(string assetPairId, string priceType, DateTime feedTime);
    }
}