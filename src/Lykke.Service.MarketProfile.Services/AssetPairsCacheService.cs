using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Lykke.Domain.Prices.Contracts;
using Lykke.Service.MarketProfile.Core.Domain;
using Lykke.Service.MarketProfile.Core.Services;

namespace Lykke.Service.MarketProfile.Services
{
    public class AssetPairsCacheService : IAssetPairsCacheService
    {
        private ConcurrentDictionary<string, IAssetPair> _pairs = new ConcurrentDictionary<string, IAssetPair>();

        public void InitCache(IEnumerable<IAssetPair> pairsToCache)
        {
            var entries = pairsToCache.Select(p => new KeyValuePair<string, IAssetPair>(p.Code, p));

            _pairs = new ConcurrentDictionary<string, IAssetPair>(entries);
        }

        public void UpdatePair(IQuote quote)
        {
            _pairs.AddOrUpdate(
                key: quote.AssetPair,
                addValueFactory: pairCode => AssetPair.Create(quote),
                updateValueFactory: (pairCode, pair) => pair.ProcessQuote(quote));
        }

        public IAssetPair TryGetPair(string pairCode)
        {
            _pairs.TryGetValue(pairCode, out IAssetPair pair);

            return pair;
        }

        public IAssetPair[] GetAll()
        {
            return _pairs
                .ToArray()
                .Select(x => x.Value)
                .ToArray();
        }
    }
}