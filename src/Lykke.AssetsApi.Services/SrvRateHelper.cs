using System.Threading.Tasks;
using Common;
using Lykke.AssetsApi.Core.Domain.Assets;
using Lykke.AssetsApi.Core.Services;

namespace Lykke.AssetsApi.Services
{
    public class SrvRateHelper : ISrvRatesHelper
    {
        private readonly IMarketProfileService _marketProfileService;

        public SrvRateHelper(IMarketProfileService marketProfileService)
        {
            _marketProfileService = marketProfileService;
        }

        public async Task<double> GetRate(string neededAssetId, IAssetPair assetPair)
        {
            var rates = await _marketProfileService.GetFeedDataAsync(assetPair.Id);
            return GetRate(neededAssetId, assetPair, rates.Ask);
        }

        public double GetRate(string neededAssetId, IAssetPair assetPair, double price)
        {
            var inverted = assetPair.IsInverted(neededAssetId);
            int accuracy = inverted ? assetPair.Accuracy : assetPair.InvertedAccuracy;
            var rate = inverted ? price : 1 / price;

            return rate.TruncateDecimalPlaces(accuracy);
        }
    }
}