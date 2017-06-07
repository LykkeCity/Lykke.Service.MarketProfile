using System;

namespace Lykke.MarketProfileService.Core.Domain.Feed
{
    public interface IFeedData
    {
        string Asset { get; }
        DateTime DateTime { get; }
        double Bid { get; }
        double Ask { get; }
    }
}