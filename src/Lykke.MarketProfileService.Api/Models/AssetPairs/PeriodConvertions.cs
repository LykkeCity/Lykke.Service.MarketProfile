using System;
using Lykke.Domain.Prices;

namespace Lykke.MarketProfileService.Api.Models.AssetPairs
{
    public static class PeriodConvertions
    {
        public static TimeInterval ToDomainModel(this Period candleType)
        {
            switch (candleType)
            {
                case Period.Sec:
                    return TimeInterval.Sec;
                case Period.Minute:
                    return TimeInterval.Minute;
                case Period.Hour:
                    return TimeInterval.Hour;
                case Period.Day:
                    return TimeInterval.Day;
                case Period.Month:
                    return TimeInterval.Month;
                default:
                    throw new ArgumentOutOfRangeException(nameof(candleType), candleType, null);
            }
        }
    }
}