using Lykke.Service.MarketProfile.Core.Domain;

namespace Lykke.Service.MarketProfile.Models.MarketProfile
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