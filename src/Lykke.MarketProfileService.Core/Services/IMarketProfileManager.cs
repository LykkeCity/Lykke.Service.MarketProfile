using Autofac;
using Lykke.MarketProfileService.Core.Domain;

namespace Lykke.MarketProfileService.Core.Services
{
    public interface IMarketProfileManager : IStartable
    {
        IAssetPair TryGetPair(string pairCode);
        IAssetPair[] GetAllPairs();
    }
}