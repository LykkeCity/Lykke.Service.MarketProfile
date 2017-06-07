using System.Collections.Generic;

namespace Lykke.MarketProfileService.Core.Domain.Feed
{
    public class MarketProfile
    {
        public IEnumerable<IFeedData> Profile { get; set; }
    }
}