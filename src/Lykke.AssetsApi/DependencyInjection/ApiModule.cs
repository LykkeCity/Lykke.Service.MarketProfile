using System;
using System.Collections.Generic;
using Autofac;
using Lykke.AssetsApi.Core;

namespace Lykke.AssetsApi.DependencyInjection
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

            // Ignore case of asset in asset connections
            _settings.CandleHistoryAssetConnections = new Dictionary<string, string>(_settings.CandleHistoryAssetConnections, StringComparer.OrdinalIgnoreCase);
        }
    }
}