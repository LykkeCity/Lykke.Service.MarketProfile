using System;
using Lykke.Common.ApiLibrary.Middleware;
using Lykke.Sdk;
using Lykke.Service.MarketProfile.Core;
using Lykke.Service.MarketProfile.Models;
using Lykke.Service.MarketProfile.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.MarketProfile
{
    public class Startup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildServiceProvider<AppSettings>(options =>
            {
                options.SwaggerOptions = new LykkeSwaggerOptions
                {
                    ApiTitle = "Lykke Market Profile",
                    ApiVersion = "v1",
                };

                options.Logs = logs =>
                {
                    logs.AzureTableName = "MarketProfileService";
                    logs.AzureTableConnectionStringResolver = s => s.MarketProfileService.Db.LogsConnectionString;
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseLykkeMiddleware(ex => new ErrorModel
            {
                Code = ErrorCode.RuntimeProblem,
                Message = "Technical problem"
            });

            app.UseStaticFiles();
            app.UseMvc();

            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swagger, httpReq) => swagger.Host = httpReq.Host.Value);
            });
            app.UseSwaggerUI(x =>
            {
                x.RoutePrefix = "swagger/ui";
                x.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            });

        }
    }
}
