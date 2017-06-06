namespace Lykke.AssetsApi.Core.Domain.Accounts
{
    public interface IWallet
    {
        double Balance { get; }
        string AssetId { get; }
    }
}