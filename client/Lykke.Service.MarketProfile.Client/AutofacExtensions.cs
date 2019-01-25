using System;
using System.Net.Http;
using Autofac;
using JetBrains.Annotations;

namespace Lykke.Service.MarketProfile.Client
{
    [PublicAPI]
    public static class AutofacExtensions
    {
        public static void RegisterMarketProfileClient(this ContainerBuilder builder, string url)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            builder.RegisterInstance<ILykkeMarketProfile>(new LykkeMarketProfile(new Uri(url), new HttpClient()));
        }
    }
}
