using System;
using System.IO;
using System.Net.Http;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AzureStorage.Tables;
using Common.Log;
using Lykke.AzureQueueIntegration;
using Lykke.Logs;
using Lykke.MarketProfileService.Api.DependencyInjection;
using Lykke.MarketProfileService.Api.Middleware;
using Lykke.MarketProfileService.Core;
using Lykke.SettingsReader;
using Lykke.SlackNotification.AzureQueue;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.Swagger.Model;

namespace Lykke.MarketProfileService.Api
{
    public class Startup
    {
        public IHostingEnvironment HostingEnvironment { get; }
        public IContainer ApplicationContainer { get; set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            HostingEnvironment = env;
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            ILog log = new LogToConsole();

            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                });

            services.AddSwaggerGen(options =>
            {
                options.SingleApiVersion(new Info
                {
                    Version = "v1",
                    Title = "Lykke Market Profile Service API"
                });
                options.DescribeAllEnumsAsStrings();

                //Determine base path for the application.
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;

                //Set the comments path for the swagger json and ui.
                var xmlPath = Path.Combine(basePath, "Lykke.MarketProfileService.Api.xml");
                options.IncludeXmlComments(xmlPath);
            });

            var settings = LoadSettings();
            var appSettings = settings.MarketProfileService;

            var slackService = services.UseSlackNotificationsSenderViaAzureQueue(new AzureQueueSettings
            {
                ConnectionString = settings.SlackNotifications.AzureQueue.ConnectionString,
                QueueName = settings.SlackNotifications.AzureQueue.QueueName
            }, log);

            if (!string.IsNullOrEmpty(appSettings.Db.LogsConnectionString))
            {
                log = new LykkeLogToAzureStorage("Lykke.MarketProfileService", new AzureTableStorage<LogEntity>(
                    appSettings.Db.LogsConnectionString, "MarketProfileServiceLogs", log), slackService);
            }

            var builder = new ContainerBuilder();

            builder.RegisterModule(new ApiModule(appSettings, log));
            builder.Populate(services);

            ApplicationContainer = builder.Build();

            return new AutofacServiceProvider(ApplicationContainer);
        }

        private static ApplicationSettings LoadSettings()
        {
            var settingsUrl = Environment.GetEnvironmentVariable("SettingsUrl");

            if (string.IsNullOrEmpty(settingsUrl))
            {
                throw new Exception("HostingEnvironment variable 'SettingsUrl' is not defined");
            }

            using (var httpClient = new HttpClient())
            {
                using (var response = httpClient.GetAsync(settingsUrl).Result)
                {
                    var settingsData = response.Content.ReadAsStringAsync().Result;

                    return SettingsProcessor.Process<ApplicationSettings>(settingsData);
                }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMiddleware<GlobalErrorHandlerMiddleware>();

            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUi();
        }
    }
}
