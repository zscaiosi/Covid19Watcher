﻿using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Covid19Watcher.Scraper.Services;
using Covid19Watcher.Application.Contracts;
using Newtonsoft.Json;

namespace Covid19Watcher.Scraper
{
    class Program
    {
        public static IConfiguration configuration;
        private static ChromeService _chrome;
        static void Main(string[] args)
        {
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR");
                
                Console.WriteLine($"Starting with args: {JsonConvert.SerializeObject(args)}");
                // Waits for execution
                MainAsync(args).Wait();

                Console.WriteLine("Finished!");
                // Tells OS process exited successfully
                Environment.Exit(0);
            }
            catch(Exception e)
            {
                Console.WriteLine($"{e.Source}: {e.Message}");
                _chrome.Close();
                Environment.Exit(1);
            }
        }
        static async Task MainAsync(string[] args)
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            // Service Provider
            IServiceProvider sp = serviceCollection.BuildServiceProvider();

            // Gets transient
            _chrome = sp.GetService<ChromeService>();
            // Runs browser
            await _chrome.RunAsync(args);
        }
        // Services DI
        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            // Build configuration
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();
            // Injects
            serviceCollection.AddSingleton<IConfiguration>(configuration);
            serviceCollection.AddHttpClient();
            // serviceCollection.AddTransient<IRestService, RestService>();
            serviceCollection.AddTransient<ChromeService>();
        }
    }
}
