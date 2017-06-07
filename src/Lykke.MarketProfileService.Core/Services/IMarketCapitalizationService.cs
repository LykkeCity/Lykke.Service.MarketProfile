using System.Threading.Tasks;

namespace Lykke.MarketProfileService.Core.Services
{
    public interface IMarketCapitalizationService
    {
        Task<double> GetCapitalization(string market);
    }
}