using System.Linq;
using AzureStorage.Tables;
using Common;
using Lykke.Domain.Prices.Repositories;
using Lykke.MarketProfileService.Api.Exceptions;
using Lykke.MarketProfileService.Core;
using Lykke.MarketProfileService.Core.Domain.Accounts;
using Lykke.MarketProfileService.Core.Domain.Assets;
using Lykke.MarketProfileService.Core.Domain.Feed;
using Lykke.MarketProfileService.Core.Services;
using Lykke.MarketProfileService.Repositories.Accounts;
using Lykke.MarketProfileService.Repositories.Assets;
using Lykke.MarketProfileService.Repositories.Feed;
using Microsoft.Extensions.DependencyInjection;
using AzureRepositories.Candles;
using Lykke.MarketProfileService.Services;

namespace Lykke.MarketProfileService.Api.DependencyInjection
{
    public class ApiServicesRegistration
    {
        public static void Register(ApplicationSettings settings, IServiceCollection services)
        {
            services.AddSingleton<IAssetsRepository>(
                new AssetsRepository(new AzureTableStorage<AssetEntity>(settings.MarketProfileService.Db.DictsConnString, "Dictionaries",
                    null)));

            services.AddSingleton<IAssetPairsRepository>(
                new AssetPairsRepository(new AzureTableStorage<AssetPairEntity>(settings.MarketProfileService.Db.DictsConnString, "Dictionaries",
                    null)));

            services.AddSingleton<IAssetPairBestPriceRepository>(
                new AssetPairBestPriceRepository(new AzureTableStorage<FeedDataEntity>(settings.MarketProfileService.Db.HLiquidityConnString,
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
                new FeedHistoryRepository(new AzureTableStorage<FeedHistoryEntity>(settings.MarketProfileService.Db.HLiquidityConnString,
                    "FeedHistory", null)));

            services.AddSingleton<IWalletsRepository>(
                new WalletsRepository(new AzureTableStorage<WalletEntity>(settings.MarketProfileService.Db.BalancesInfoConnString,
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
            services.AddTransient<IMarketProfileService, Services.MarketProfileService>();
            services.AddTransient<ISrvRatesHelper, SrvRateHelper>();
        }
    }
}