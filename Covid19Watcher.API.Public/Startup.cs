using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Covid19Watcher.API.Public.Data.MongoDB.DAOs;
using Covid19Watcher.API.Public.Data.MongoDB.Repositories;
using Covid19Watcher.API.Public.Interfaces;
using Covid19Watcher.API.Public.Services;
using Covid19Watcher.API.Public.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Covid19Watcher.API.Public
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Sets up Mongodb configurations and injects by DI Container
            services.Configure<MongoDBSettings>(
                Configuration.GetSection(nameof(MongoDBSettings))
            );
            services.AddSingleton<IMongoDBSettings, MongoDBSettings>(provider => provider.GetRequiredService<IOptions<MongoDBSettings>>().Value);
            // Injects other services
            services.AddSingleton<INotificationsDAO, NotificationsDAO>();
            services.AddSingleton<INotificationsRepository, NotificationsRepository>();
            services.AddSingleton<INotificationsService, NotificationsService>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
