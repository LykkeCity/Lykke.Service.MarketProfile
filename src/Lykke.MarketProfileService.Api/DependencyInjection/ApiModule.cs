using Autofac;
using AzureStorage.Tables;
using Lykke.MarketProfileService.Core;
using Lykke.MarketProfileService.Core.Domain;
using Lykke.MarketProfileService.Core.Services;
using Lykke.MarketProfileService.Repositories;
using Lykke.MarketProfileService.Repositories.Entities;
using Lykke.MarketProfileService.Services;

namespace Lykke.MarketProfileService.Api.DependencyInjection
{
    public class ApiModule : Module
    {
        private readonly ApplicationSettings _settings;

        public ApiModule(ApplicationSettings settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_settings)
                .SingleInstance();

            builder.Register<IAssetPairsRepository>(
                x => new AssetPairRepository(
                    new AzureTableStorage<AssetPairEntity>(_settings.MarketProfileService.Db.ConnectionString,
                        "AssetPairs", null)));

            builder.RegisterType<AssetPairsCacheService>().As<IAssetPairsCacheService>();

            builder.RegisterType<MarketProfileManager>()
                .As<IMarketProfileManager>()
                .As<IStartable>()
                .SingleInstance();
        }
    }
}