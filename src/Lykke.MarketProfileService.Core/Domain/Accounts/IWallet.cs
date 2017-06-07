namespace Lykke.MarketProfileService.Core.Domain.Accounts
{
    public interface IWallet
    {
        double Balance { get; }
        string AssetId { get; }
    }
}