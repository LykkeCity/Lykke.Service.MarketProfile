using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.MarketProfileService.Core.Domain
{
    public interface IAssetPairsRepository
    {
        Task AddAsync(IAssetPair pair);
        Task<IEnumerable<IAssetPair>> GetAllAsync();
        Task AddOrUpdateAllAsync(IEnumerable<IAssetPair> pairs);
    }
}