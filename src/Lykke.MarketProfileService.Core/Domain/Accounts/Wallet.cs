using Lykke.MarketProfileService.Core.Domain.Assets;

namespace Lykke.MarketProfileService.Core.Domain.Accounts
{
    public class Wallet : IWallet
    {
        public string AssetId { get; set; }
        public double Balance { get; set; }

        public static Wallet Create(IAsset asset, double balance = 0)
        {
            return new Wallet
            {
                AssetId = asset.Id,
                Balance = balance
            };
        }
    }
}