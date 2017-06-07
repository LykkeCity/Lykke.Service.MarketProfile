using Lykke.MarketProfileService.Core.Domain;

namespace Lykke.MarketProfileService.Api.Models.MarketProfile
{
    public static class AssetPairModelConvertions
    {
        public static AssetPairModel ToApiModel(this IAssetPair pair)
        {
            return new AssetPairModel
            {
                AssetPair = pair.Code,
                BidPrice = pair.BidPrice,
                AskPrice = pair.AskPrice,
                BidPriceTimestamp = pair.BidPriceTimestamp,
                AskPriceTimestamp = pair.AskPriceTimestamp
            };
        }
    }
}