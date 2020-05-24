using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Covid19Watcher.Application.Data.MongoDB.DAOs;
using Covid19Watcher.Application.Data.MongoDB.Repositories;
using Covid19Watcher.Application.Interfaces;
using Covid19Watcher.Application.Services;
using Covid19Watcher.Application.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
            services.AddCors();
            services.AddSingleton<IConfiguration>(Configuration);
            // Sets up Mongodb configurations and injects by DI Container
            services.Configure<MongoDBSettings>(
                Configuration.GetSection(nameof(MongoDBSettings))
            );
            services.AddSingleton<IMongoDBSettings, MongoDBSettings>(provider => provider.GetRequiredService<IOptions<MongoDBSettings>>().Value);
            // Injects other services
            services.AddSingleton<INotificationsDAO, NotificationsDAO>();
            services.AddSingleton<INotificationsRepository, NotificationsRepository>();
            services.AddSingleton<INotificationsService, NotificationsService>();
            // Gets key from config
            var key = Encoding.ASCII.GetBytes(Configuration.GetSection("Credentials").GetSection("key").Value);

            services.AddAuthentication(x =>
            {	
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opt => {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "Covid19Watcher",
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
                // Define the events handler
                opt.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine("Token inválido..:. " + context.Exception.Message);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("Token válido...: " + context.SecurityToken);
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "COVID-19 Notifications", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // It is a public REST API
            app.UseCors(opt => {
                opt.AllowAnyHeader();
                opt.AllowAnyOrigin();
                opt.AllowAnyMethod();
            });

            // app.UseHttpsRedirection();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "COVID-19 Notifications");
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
