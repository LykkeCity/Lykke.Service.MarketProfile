using System.Threading.Tasks;
using Lykke.AssetsApi.Core.Domain.Assets;

namespace Lykke.AssetsApi.Core.Services
{
    public interface ISrvRatesHelper
    {
        Task<double> GetRate(string neededAssetId, IAssetPair assetPair);
        double GetRate(string neededAssetId, IAssetPair assetPair, double price);
    }
}