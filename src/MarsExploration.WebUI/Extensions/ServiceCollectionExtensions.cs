using System;
using System.Linq;
using MarsExploration.WebUI.Configurations;
using MarsExploration.WebUI.Gateways;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MarsExploration.WebUI.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        private const string DefaultConfigurationSection = "MarsExplorationApi";

        public static IServiceCollection AddMarsExplorationService(this IServiceCollection services)
            => services.AddMarsExplorationService(DefaultConfigurationSection);

        public static IServiceCollection AddMarsExplorationService(this IServiceCollection services, string configurationSection)
            => InjectMarsExplorationService(services, configurationSection);

        private static IServiceCollection InjectMarsExplorationService(IServiceCollection services, string configurationSection)
        {
            if(string.IsNullOrEmpty(configurationSection))
                throw new ArgumentNullException(nameof(configurationSection));
            
            if(services.Any(sd => sd.ServiceType == typeof(IMarsExplorationServiceGateway)))
                return services;

            services.AddSingleton(sp => BindConfiguration(sp, new MarsExplorationServiceConfiguration()));
            services.AddHttpClient(HttpClientDefaults.Name, (sp, httpClient) => {
                var configuration = sp.GetRequiredService<MarsExplorationServiceConfiguration>();
                var secureUrl = configuration.Urls.FirstOrDefault(uri => uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase));

                httpClient.BaseAddress = secureUrl ?? configuration.Urls.First();
            });

            AddGateways(services);
            
            return services;

            // Local function to avoid messy private function with multiple parameters
            MarsExplorationServiceConfiguration BindConfiguration(IServiceProvider serviceProvider, MarsExplorationServiceConfiguration explorationServiceConfiguration)
            {
                var appConfig = serviceProvider.GetRequiredService<IConfiguration>();
                appConfig.Bind(configurationSection, explorationServiceConfiguration);

                ValidateConfiguration(explorationServiceConfiguration);

                return explorationServiceConfiguration;
            }
        }

        private static void AddGateways(IServiceCollection services)
        {
            services.AddTransient<IMarsExplorationServiceGateway, MarsExplorationServiceGateway>();
        }

        private static void ValidateConfiguration(MarsExplorationServiceConfiguration configuration)
        {
            if(configuration.Urls == null || !configuration.Urls.Any())
                throw new InvalidOperationException("Mars Exploration WebService configuration is not valid");
        }
    }
}