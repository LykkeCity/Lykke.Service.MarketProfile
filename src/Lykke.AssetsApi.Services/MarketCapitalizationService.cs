using System;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Lykke.AssetsApi.Core;
using Lykke.AssetsApi.Core.Domain.Accounts;
using Lykke.AssetsApi.Core.Domain.Assets;
using Lykke.AssetsApi.Core.Services;
using Microsoft.Extensions.Caching.Memory;

namespace Lykke.AssetsApi.Services
{
    public class MarketCapitalizationService : IMarketCapitalizationService
    {
        private class CacheRecord
        {
            public DateTime Dt { get; set; }
            public string AssetId { get; set; }
            public double Amount { get; set; }
        }

        private const string MarketCapitalizationCacheKey = "_MarketCapital_{0}";
        private readonly TimeSpan _cacheExpTime = TimeSpan.FromMinutes(10);

        private readonly IWalletsRepository _walletsRepository;
        private readonly IMemoryCache _memCache;
        private readonly CachedDataDictionary<string, IAssetPair> _assetPairsDict;
        private readonly CachedDataDictionary<string, IAsset> _assetsDict;
        private readonly ISrvRatesHelper _srvRatesHelper;

        public MarketCapitalizationService(
            IWalletsRepository walletsRepository,
            IMemoryCache memCache,
            CachedDataDictionary<string, IAssetPair> assetPairsDict,
            CachedDataDictionary<string, IAsset> assetsDict,
            ISrvRatesHelper srvRatesHelper)
        {
            _walletsRepository = walletsRepository;
            _memCache = memCache;
            _assetPairsDict = assetPairsDict;
            _assetsDict = assetsDict;
            _srvRatesHelper = srvRatesHelper;
        }

        public async Task<double> GetCapitalization(string market)
        {
            double rate = 1;
            if (market != LykkeConstants.LykkeAssetId)
            {
                var assetPairs = await _assetPairsDict.Values();
                var pair = assetPairs.PairWithAssets(LykkeConstants.LykkeAssetId, market);

                if (pair == null)
                    return 0;

                rate = await _srvRatesHelper.GetRate(market, pair);
            }

            CacheRecord record;
            var asset = await _assetsDict.GetItemAsync(market);

            var cacheKey = GetMarketCapitalizationCacheKey();

            if (!_memCache.TryGetValue(cacheKey, out record))
            {
                double amount = 0;

                await _walletsRepository.GetWalletsByChunkAsync(pairs =>
                {
                    var c =
                        pairs.Select(x => x.Value?.FirstOrDefault(y => y.AssetId == LykkeConstants.LykkeAssetId))
                            .Sum(x => x?.Balance ?? 0);
                    amount += c;
                    return Task.CompletedTask;
                });

                record = record ?? new CacheRecord();

                record.AssetId = LykkeConstants.LykkeAssetId;
                record.Dt = DateTime.UtcNow;
                record.Amount = amount;

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(_cacheExpTime);

                _memCache.Set(cacheKey, record, cacheEntryOptions);
            }

            return (record.Amount * rate).TruncateDecimalPlaces(asset.Accuracy);
        }

        private static string GetMarketCapitalizationCacheKey()
        {
            return string.Format(MarketCapitalizationCacheKey, LykkeConstants.LykkeAssetId);
        }
    }
}

