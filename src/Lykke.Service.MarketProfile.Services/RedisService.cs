using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Job.MarketProfile.Contract;
using Lykke.Job.MarketProfile.Contract.Extensions;
using Lykke.Service.MarketProfile.Core.Services;
using StackExchange.Redis;

namespace Lykke.Service.MarketProfile.Services
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _database;

        public RedisService(
            IDatabase database
        )
        {
            _database = database;
        }

        public async Task<AssetPairPrice> GetMarketProfileAsync(string assetPair)
        {
            var data = await _database.HashGetAllAsync(RedisKeys.GetMarketProfileKey(assetPair));

            return data.Length == 0 ? null : data.ToAssetPairPrice();
        }
        
        public async Task<List<AssetPairPrice>> GetMarketProfilesAsync()
        {
            var result = new List<AssetPairPrice>();
            List<string> assetPairs = await GetAssetPairs();

            foreach (string assetPair in assetPairs)
            {
                var marketProfile = await GetMarketProfileAsync(assetPair);

                if (marketProfile != null)
                    result.Add(marketProfile);
            }

            return result;
        }

        private async Task<List<string>> GetAssetPairs()
        {
            var assetPairs = await _database.SetMembersAsync(RedisKeys.GetAssetPairsKey());
            return assetPairs.Select(assetPair => (string) assetPair).ToList();
        }
    }
}
