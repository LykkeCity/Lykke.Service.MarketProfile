using System.Collections.Generic;

namespace Lykke.AssetsApi.Core.Domain.Feed
{
    public class MarketProfile
    {
        public IEnumerable<IFeedData> Profile { get; set; }
    }
}