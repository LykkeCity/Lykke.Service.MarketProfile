using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.MarketProfile.Core.Domain
{
    public interface IAssetPairsRepository
    {
        Task<IEnumerable<IAssetPair>> Read();
        Task Write(IEnumerable<IAssetPair> pairs);
    }
}