using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureStorage;
using Common;
using Lykke.Service.MarketProfile.Core.Domain;
using Newtonsoft.Json;

namespace Lykke.Service.MarketProfile.Repositories
{
    public class AssetPairRepository : IAssetPairsRepository
    {
        private readonly string _container;
        private readonly string _key;
        private readonly IBlobStorage _storage;

        public AssetPairRepository(IBlobStorage storage, string container, string key)
        {
            _container = container;
            _key = key;
            _storage = storage;
        }

        public async Task<IEnumerable<IAssetPair>> Read()
        {
            if (await _storage.HasBlobAsync(_container, _key))
            {
                var data = await _storage.GetAsync(_container, _key);
                var content = Encoding.UTF8.GetString(data.ToBytes());

                return JsonConvert.DeserializeObject<AssetPair[]>(content);
            }

            return Enumerable.Empty<IAssetPair>();
        }

        public async Task Write(IEnumerable<IAssetPair> pairs)
        {
            var data = JsonConvert.SerializeObject(pairs).ToUtf8Bytes();

            await _storage.SaveBlobAsync(_container, _key, data);
        }
    }
}