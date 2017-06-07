namespace Lykke.MarketProfileService.Api.Models.Assets
{
    public class AssetModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string BitcoinAssetId { get; set; }
        public string BitcoinAssetAddress { get; set; }
        public string Symbol { get; set; }
        public int Accuracy { get; set; }
    }
}