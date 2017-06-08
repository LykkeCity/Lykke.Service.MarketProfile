using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Lykke.Domain.Prices.Contracts;
using Lykke.MarketProfileService.Core.Domain;
using Lykke.MarketProfileService.Core.Services;

namespace Lykke.MarketProfileService.Services
{
    public class AssetPairsCacheService : IAssetPairsCacheService
    {
        private readonly Dictionary<string, IAssetPair> _pairs = new Dictionary<string, IAssetPair>();
        private readonly ReaderWriterLockSlim _lockSlim = new ReaderWriterLockSlim();

        public void InitCache(IEnumerable<IAssetPair> pairsToCache)
        {
            _lockSlim.EnterWriteLock();

            try
            {
                _pairs.Clear();

                foreach (var pair in pairsToCache)
                {
                    _pairs.Add(pair.Code, pair);
                }
            }
            finally
            {
                _lockSlim.ExitWriteLock();
            }
        }

        public void UpdatePair(IQuote quote)
        {
            _lockSlim.EnterWriteLock();

            try
            {
                if (!_pairs.TryGetValue(quote.AssetPair, out IAssetPair pair))
                {
                    pair = new AssetPair
                    {
                        Code = quote.AssetPair,
                        AskPriceTimestamp = DateTime.MinValue,
                        BidPriceTimestamp = DateTime.MinValue
                    };
                    _pairs.Add(quote.AssetPair, pair);
                }

                if (quote.IsBuy)
                {
                    if (pair.BidPriceTimestamp < quote.Timestamp)
                    {
                        pair.BidPrice = quote.Price;
                        pair.BidPriceTimestamp = quote.Timestamp;
                    }
                }
                else
                {
                    if (pair.AskPriceTimestamp < quote.Timestamp)
                    {
                        pair.AskPrice = quote.Price;
                        pair.AskPriceTimestamp = quote.Timestamp;
                    }
                }
            }
            finally
            {
                _lockSlim.ExitWriteLock();
            }
        }

        public IAssetPair TryGetPair(string pairCode)
        {
            _lockSlim.EnterReadLock();

            try
            {
                _pairs.TryGetValue(pairCode, out IAssetPair pair);

                return pair;
            }
            finally
            {
                _lockSlim.ExitReadLock();
            }
        }

        public IAssetPair[] GetAll()
        {
            _lockSlim.EnterReadLock();

            try
            {
                return _pairs.Select(x => x.Value).ToArray();
            }
            finally
            {
                _lockSlim.ExitReadLock();
            }
        }
    }
}