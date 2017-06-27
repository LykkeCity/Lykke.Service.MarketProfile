using Autofac;
using Lykke.Service.MarketProfile.Core.Domain;

namespace Lykke.Service.MarketProfile.Core.Services
{
    public interface IMarketProfileManager : IStartable
    {
        IAssetPair TryGetPair(string pairCode);
        IAssetPair[] GetAllPairs();
    }
}