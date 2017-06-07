using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Common;
using Lykke.MarketProfileService.Core.Domain;
using Lykke.MarketProfileService.Repositories.Entities;

namespace Lykke.MarketProfileService.Repositories
{
    public class AssetPairRepository : IAssetPairsRepository
    {
        private readonly INoSQLTableStorage<AssetPairEntity> _tableStorage;

        public AssetPairRepository(INoSQLTableStorage<AssetPairEntity> tableStorage)
        {
            _tableStorage = tableStorage;
        }

        public async Task AddAsync(IAssetPair pair)
        {
            await _tableStorage.InsertOrReplaceAsync(AssetPairEntity.Create(pair));
        }

        public async Task<IEnumerable<IAssetPair>> GetAllAsync()
        {
            return (await _tableStorage.GetDataAsync()).Select(AssetPair.Create);
        }

        public async Task AddOrUpdateAllAsync(IEnumerable<IAssetPair> pairs)
        {
            foreach (var chunk in pairs.ToChunks(50))
            {
                await _tableStorage.InsertOrReplaceBatchAsync(chunk.Select(AssetPairEntity.Create));
            }
        }
    }
}