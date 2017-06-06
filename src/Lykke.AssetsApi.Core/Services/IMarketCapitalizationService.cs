using System.Threading.Tasks;

namespace Lykke.AssetsApi.Core.Services
{
    public interface IMarketCapitalizationService
    {
        Task<double> GetCapitalization(string market);
    }
}