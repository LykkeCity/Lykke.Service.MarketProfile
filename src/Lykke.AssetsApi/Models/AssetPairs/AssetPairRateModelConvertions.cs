using System.Collections.Generic;
using System.Linq;
using Lykke.AssetsApi.Core.Domain.Feed;

namespace Lykke.AssetsApi.Models.AssetPairs
{
    public static class AssetPairRateModelConvertions
    {
        public static IEnumerable<AssetPairRateModel> ToApiModel(this MarketProfile marketProfile)
        {
            return marketProfile.Profile.Select(x => x.ToApiModel());
        }

        public static AssetPairRateModel ToApiModel(this IFeedData feedData)
        {
            return new AssetPairRateModel
            {
                Ask = feedData.Ask,
                Bid = feedData.Bid,
                Id = feedData.Asset
            };
        }
    }
}