using System.Collections.Generic;
using Lykke.Domain.Prices.Contracts;
using Lykke.MarketProfileService.Core.Domain;

namespace Lykke.MarketProfileService.Core.Services
{
    public interface IAssetPairsCacheService
    {
        void InitCache(IEnumerable<IAssetPair> pairsToCache);
        void UpdatePair(IQuote quote);
        IAssetPair TryGetPair(string pairCode);
        IAssetPair[] GetAll();
    }
}