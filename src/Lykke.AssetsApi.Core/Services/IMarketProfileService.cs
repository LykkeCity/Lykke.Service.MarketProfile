using System.Threading.Tasks;
using Lykke.AssetsApi.Core.Domain.Feed;

namespace Lykke.AssetsApi.Core.Services
{
    public interface IMarketProfileService
    {
        Task<MarketProfile> GetMarketProfileAsync();
        Task<IFeedData> GetFeedDataAsync(string assetPairId);
    }
}