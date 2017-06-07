using Autofac;
using AzureStorage.Blob;
using Common.Log;
using Lykke.MarketProfileService.Core;
using Lykke.MarketProfileService.Core.Domain;
using Lykke.MarketProfileService.Core.Services;
using Lykke.MarketProfileService.Repositories;
using Lykke.MarketProfileService.Services;

namespace Lykke.MarketProfileService.Api.DependencyInjection
{
    public class ApiModule : Module
    {
        private readonly ApplicationSettings.MarketProfileServiceSettings _settings;
        private readonly ILog _log;

        public ApiModule(ApplicationSettings.MarketProfileServiceSettings settings, ILog log)
        {
            _settings = settings;
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_log).SingleInstance();

            builder.RegisterInstance(_settings).SingleInstance();

            builder.Register<IAssetPairsRepository>(
                x => new AssetPairRepository(
                        new AzureBlobStorage(_settings.Db.CachePersistenceConnectionString),
                        "AssetPairs",
                        "Instance"));

            builder.RegisterType<AssetPairsCacheService>().As<IAssetPairsCacheService>();

            builder.RegisterType<MarketProfileManager>()
                .As<IMarketProfileManager>()
                .As<IStartable>()
                .SingleInstance();
        }
    }
}