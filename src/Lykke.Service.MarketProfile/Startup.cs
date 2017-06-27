using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AzureStorage.Tables;
using Common.Log;
using Lykke.AzureQueueIntegration;
using Lykke.Common.ApiLibrary.Middleware;
using Lykke.Common.ApiLibrary.Swagger;
using Lykke.Logs;
using Lykke.Service.MarketProfile.Core;
using Lykke.Service.MarketProfile.DependencyInjection;
using Lykke.Service.MarketProfile.Models;
using Lykke.SettingsReader;
using Lykke.SlackNotification.AzureQueue;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lykke.Service.MarketProfile
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
            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                });

            services.AddSwaggerGen(options =>
            {
                options.DefaultLykkeConfiguration("v1", "Lykke Market Profile");
            });

            var settings = HttpSettingsLoader.Load<ApplicationSettings>();
            var log = CreateLog(services, settings);
            var appSettings = settings.MarketProfileService;
            var builder = new ContainerBuilder();

            builder.RegisterModule(new ApiModule(appSettings, log));
            builder.Populate(services);

            ApplicationContainer = builder.Build();

            return new AutofacServiceProvider(ApplicationContainer);
        }

        private static ILog CreateLog(IServiceCollection services, ApplicationSettings settings)
        {
            var appSettings = settings.MarketProfileService;

            LykkeLogToAzureStorage logToAzureStorage = null;
            var logToConsole = new LogToConsole();
            var logAggregate = new LogAggregate();

            logAggregate.AddLogger(logToConsole);

            if (!string.IsNullOrEmpty(appSettings.Db.LogsConnectionString) &&
                !(appSettings.Db.LogsConnectionString.StartsWith("${") && appSettings.Db.LogsConnectionString.EndsWith("}")))
            {
                logToAzureStorage = new LykkeLogToAzureStorage("Lykke.Service.MarketProfile", new AzureTableStorage<LogEntity>(
                    appSettings.Db.LogsConnectionString, "MarketProfileService", logToConsole));

                logAggregate.AddLogger(logToAzureStorage);
            }

            var log = logAggregate.CreateLogger();

            var slackService = services.UseSlackNotificationsSenderViaAzureQueue(new AzureQueueSettings
            {
                ConnectionString = settings.SlackNotifications.AzureQueue.ConnectionString,
                QueueName = settings.SlackNotifications.AzureQueue.QueueName
            }, log);

            logToAzureStorage?.SetSlackNotification(slackService);
            return log;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseLykkeMiddleware(Constants.ComponentName, ex => new ErrorModel
            {
                Code = ErrorCode.RuntimeProblem,
                Message = "Technical problem"
            });

            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUi();
        }
    }
}
