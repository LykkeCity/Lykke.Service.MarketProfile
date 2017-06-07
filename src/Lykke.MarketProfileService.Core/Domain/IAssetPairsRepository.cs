using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.MarketProfileService.Core.Domain
{
    public interface IAssetPairsRepository
    {
        Task<IEnumerable<IAssetPair>> Read();
        Task Write(IEnumerable<IAssetPair> pairs);
    }
}