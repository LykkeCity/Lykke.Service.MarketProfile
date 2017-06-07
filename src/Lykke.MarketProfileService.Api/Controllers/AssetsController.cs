using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Lykke.MarketProfileService.Api.Models.Assets;
using Lykke.MarketProfileService.Core.Domain.Assets;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.MarketProfileService.Api.Controllers
{
    [Route("api/[controller]")]
    public class AssetsController
    {
        private readonly CachedDataDictionary<string, IAsset> _assetsDict;

        public AssetsController(CachedDataDictionary<string, IAsset> assetsDict)
        {
            _assetsDict = assetsDict;
        }

        /// <summary>
        /// Get assets dictionary
        /// </summary>
        [HttpGet("dictionary")]
        public async Task<IEnumerable<AssetModel>> GetDictionary()
        {
            var assets = (await _assetsDict.Values())
                .Where(x => !x.IsDisabled)
                .Select(x => new AssetModel
                {
                    Accuracy = x.Accuracy,
                    BitcoinAssetAddress = x.AssetAddress,
                    BitcoinAssetId = x.BlockChainAssetId,
                    Id = x.Id,
                    Name = x.Name,
                    Symbol = x.Symbol
                });

            return assets;
        }
    }
}