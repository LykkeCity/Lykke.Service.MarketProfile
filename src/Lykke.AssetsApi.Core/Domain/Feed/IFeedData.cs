using System;

namespace Lykke.AssetsApi.Core.Domain.Feed
{
    public interface IFeedData
    {
        string Asset { get; }
        DateTime DateTime { get; }
        double Bid { get; }
        double Ask { get; }
    }
}