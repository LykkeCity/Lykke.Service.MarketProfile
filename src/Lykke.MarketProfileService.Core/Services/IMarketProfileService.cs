using System.Threading.Tasks;
using Lykke.MarketProfileService.Core.Domain.Feed;

namespace Lykke.MarketProfileService.Core.Services
{
    public interface IMarketProfileService
    {
        Task<MarketProfile> GetMarketProfileAsync();
        Task<IFeedData> GetFeedDataAsync(string assetPairId);
    }
}