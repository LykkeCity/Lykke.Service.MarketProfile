using Autofac;
using AzureStorage.Blob;
using Common.Log;
using Lykke.Service.MarketProfile.Core;
using Lykke.Service.MarketProfile.Core.Domain;
using Lykke.Service.MarketProfile.Core.Services;
using Lykke.Service.MarketProfile.Repositories;
using Lykke.Service.MarketProfile.Services;
using Lykke.SettingsReader;

namespace Lykke.Service.MarketProfile.DependencyInjection
{
    public class ApiModule : Module
    {
        private readonly IReloadingManager<ApplicationSettings> _appSettings;
        private readonly ILog _log;

        public ApiModule(IReloadingManager<ApplicationSettings> appSettings, ILog log)
        {
            _appSettings = appSettings;
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_log).SingleInstance();

            builder.RegisterInstance(_appSettings.CurrentValue.MarketProfileService).SingleInstance();

            builder.Register<IAssetPairsRepository>(
                x => new AssetPairRepository(AzureBlobStorage.Create(_appSettings.ConnectionString(o => o.MarketProfileService.Db.CachePersistenceConnectionString)),
                        "assetpairs",
                        "Instance"));

            builder.RegisterType<AssetPairsCacheService>().As<IAssetPairsCacheService>();

            builder.RegisterType<MarketProfileManager>()
                .As<IMarketProfileManager>()
                .As<IStartable>()
                .SingleInstance();
        }
    }
}