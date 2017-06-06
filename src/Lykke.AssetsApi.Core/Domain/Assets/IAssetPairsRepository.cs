using System.Threading.Tasks;
using System.Collections.Generic;

namespace Lykke.AssetsApi.Core.Domain.Assets
{
    public interface IAssetPairsRepository
    {
        Task<IEnumerable<IAssetPair>> GetAllAsync();
        Task<IAssetPair> GetAsync(string id);
        Task AddAsync(IAssetPair assetPair);
        Task EditAsync(string id, IAssetPair assetPair);

    }
}