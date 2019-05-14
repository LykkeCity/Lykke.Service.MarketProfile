using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Job.MarketProfile.Contract;

namespace Lykke.Service.MarketProfile.Core.Services
{
    public interface IRedisService
    {
        Task<AssetPairPrice> GetMarketProfileAsync(string assetPair);
        Task<List<AssetPairPrice>> GetMarketProfilesAsync();
    }
}
