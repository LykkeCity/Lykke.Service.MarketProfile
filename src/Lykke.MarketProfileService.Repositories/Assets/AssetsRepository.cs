using System.Collections.Generic;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.MarketProfileService.Core.Domain.Assets;

namespace Lykke.MarketProfileService.Repositories.Assets
{
    public class AssetsRepository : IAssetsRepository
    {
        private readonly INoSQLTableStorage<AssetEntity> _tableStorage;

        public AssetsRepository(INoSQLTableStorage<AssetEntity> tableStorage)
        {
            _tableStorage = tableStorage;
        }

        public Task RegisterAssetAsync(IAsset asset)
        {
            var newAsset = AssetEntity.Create(asset);
            return _tableStorage.InsertAsync(newAsset);
        }

        public async Task EditAssetAsync(string id, IAsset asset)
        {
            await _tableStorage.DeleteAsync(AssetEntity.GeneratePartitionKey(), AssetEntity.GenerateRowKey(id));
            await RegisterAssetAsync(asset);
        }
        
        public async Task<IEnumerable<IAsset>> GetAssetsAsync()
        {
            var partitionKey = AssetEntity.GeneratePartitionKey();
            return await _tableStorage.GetDataAsync(partitionKey);
        }

        public async Task<IAsset> GetAssetAsync(string id)
        {
            var partitionKey = AssetEntity.GeneratePartitionKey();
            var rowKey = AssetEntity.GenerateRowKey(id);

            return await _tableStorage.GetDataAsync(partitionKey, rowKey);
        }

        public async Task<IEnumerable<IAsset>> GetAssetsForCategoryAsync(string category)
        {
            var partitionKey = AssetEntity.GeneratePartitionKey();
            return await _tableStorage.GetDataAsync(partitionKey, x => x.CategoryId == category);
        }

        public async Task SetDisabled(string id, bool value)
        {
            await _tableStorage.ReplaceAsync(AssetEntity.GeneratePartitionKey(), AssetEntity.GenerateRowKey(id),
                assetEntity =>
                {
                    assetEntity.IsDisabled = value;
                    return assetEntity;
                });
        }
    }
}