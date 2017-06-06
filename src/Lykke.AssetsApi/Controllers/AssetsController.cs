using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Lykke.AssetsApi.Models.Assets;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Lykke.AssetsApi.Core.Domain.Assets;

namespace Lykke.AssetsApi.Controllers
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