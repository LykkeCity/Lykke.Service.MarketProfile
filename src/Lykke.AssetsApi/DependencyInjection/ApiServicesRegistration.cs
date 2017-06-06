using System.Linq;
using AzureRepositories.Candles;
using AzureStorage.Tables;
using Common;
using Lykke.AssetsApi.Core;
using Lykke.AssetsApi.Core.Domain.Accounts;
using Lykke.AssetsApi.Core.Domain.Assets;
using Lykke.AssetsApi.Core.Domain.Feed;
using Lykke.AssetsApi.Core.Services;
using Lykke.AssetsApi.Exceptions;
using Lykke.AssetsApi.Repositories.Accounts;
using Lykke.AssetsApi.Repositories.Assets;
using Lykke.AssetsApi.Repositories.Feed;
using Lykke.AssetsApi.Services;
using Lykke.Domain.Prices.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.AssetsApi.DependencyInjection
{
    public class ApiServicesRegistration
    {
        public static void Register(ApplicationSettings settings, IServiceCollection services)
        {
            services.AddSingleton<IAssetsRepository>(
                new AssetsRepository(new AzureTableStorage<AssetEntity>(settings.AssetsApi.Db.DictsConnString, "Dictionaries",
                    null)));

            services.AddSingleton<IAssetPairsRepository>(
                new AssetPairsRepository(new AzureTableStorage<AssetPairEntity>(settings.AssetsApi.Db.DictsConnString, "Dictionaries",
                    null)));

            services.AddSingleton<IAssetPairBestPriceRepository>(
                new AssetPairBestPriceRepository(new AzureTableStorage<FeedDataEntity>(settings.AssetsApi.Db.HLiquidityConnString,
                    "MarketProfile", null)));

            services.AddSingleton<ICandleHistoryRepository>(serviceProvider => new CandleHistoryRepositoryResolver((asset, tableName) =>
            {
                if (!settings.CandleHistoryAssetConnections.TryGetValue(asset, out string connString) || string.IsNullOrEmpty(connString))
                {
                    throw new AppSettingException(string.Format("Connection string for asset pair '{0}' is not specified.", asset));
                }

                return new AzureTableStorage<CandleTableEntity>(connString, tableName, null);
            }));

            services.AddSingleton<IFeedHistoryRepository>(
                new FeedHistoryRepository(new AzureTableStorage<FeedHistoryEntity>(settings.AssetsApi.Db.HLiquidityConnString,
                    "FeedHistory", null)));

            services.AddSingleton<IWalletsRepository>(
                new WalletsRepository(new AzureTableStorage<WalletEntity>(settings.AssetsApi.Db.BalancesInfoConnString,
                    "Accounts", null)));

            services.AddSingleton(x =>
            {
                var assetPairsRepository = (IAssetPairsRepository)x.GetService(typeof(IAssetPairsRepository));
                return new CachedDataDictionary<string, IAssetPair>(
                    async () => (await assetPairsRepository.GetAllAsync()).ToDictionary(itm => itm.Id));
            });

            services.AddSingleton(x =>
            {
                var assetsRepo = (IAssetsRepository)x.GetService(typeof(IAssetsRepository));
                return new CachedDataDictionary<string, IAsset>(
                    async () => (await assetsRepo.GetAssetsAsync()).ToDictionary(itm => itm.Id));
            });

            services.AddTransient<IMarketCapitalizationService, MarketCapitalizationService>();
            services.AddTransient<IMarketProfileService, MarketProfileService>();
            services.AddTransient<ISrvRatesHelper, SrvRateHelper>();
        }
    }
}