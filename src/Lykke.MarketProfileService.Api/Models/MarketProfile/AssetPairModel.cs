using System;
using System.ComponentModel.DataAnnotations;

namespace Lykke.MarketProfileService.Api.Models.MarketProfile
{
    public class AssetPairModel
    {
        [Required]
        public string AssetPair { get; set; }
        [Required]
        public double BidPrice { get; set; }
        [Required]
        public double AskPrice { get; set; }
        [Required]
        public DateTime BidPriceTimestamp { get; set; }
        [Required]
        public DateTime AskPriceTimestamp { get; set; }
    }
}