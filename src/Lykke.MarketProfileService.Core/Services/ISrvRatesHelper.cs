using System.Threading.Tasks;
using Lykke.MarketProfileService.Core.Domain.Assets;

namespace Lykke.MarketProfileService.Core.Services
{
    public interface ISrvRatesHelper
    {
        Task<double> GetRate(string neededAssetId, IAssetPair assetPair);
        double GetRate(string neededAssetId, IAssetPair assetPair, double price);
    }
}