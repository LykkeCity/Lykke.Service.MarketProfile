using System.Collections.Generic;
using System.Linq;
using Lykke.MarketProfileService.Core.Domain.Assets;

namespace Lykke.MarketProfileService.Api.Models.AssetPairs
{
    public static class AssetPairModelConvertions
    {
        public static IEnumerable<AssetPairModel> ToApiModel(this IEnumerable<IAssetPair> assetPairs)
        {
            return assetPairs.Select<IAssetPair, AssetPairModel>(x => x.ToApiModel());
        }

        public static AssetPairModel ToApiModel(this IAssetPair assetPair)
        {
            return new AssetPairModel
            {
                Accuracy = assetPair.Accuracy,
                BaseAssetId = assetPair.BaseAssetId,
                Id = assetPair.Id,
                InvertedAccuracy = assetPair.InvertedAccuracy,
                Name = assetPair.Name,
                QuotingAssetId = assetPair.QuotingAssetId
            };
        }
    }
}