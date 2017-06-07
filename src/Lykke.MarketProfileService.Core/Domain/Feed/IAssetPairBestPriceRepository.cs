using System.Threading.Tasks;

namespace Lykke.MarketProfileService.Core.Domain.Feed
{
    public interface IAssetPairBestPriceRepository
    {
        Task<MarketProfile> GetAsync();
        Task<IFeedData> GetAsync(string assetPairId);

        Task SaveAsync(IFeedData feedData);
    }
}