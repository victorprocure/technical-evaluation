using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MarsExploration.Core;
using MarsExploration.WebUI.Configurations;
using MarsExploration.WebUI.Extensions;
using Microsoft.Extensions.Logging;

namespace MarsExploration.WebUI.Gateways
{
    public class MarsExplorationServiceGateway : IMarsExplorationServiceGateway
    {
        private const string ExplorationEndpoint = "exploration";

        private readonly ILogger<MarsExplorationServiceGateway> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        private HttpClient _httpClient;

        public MarsExplorationServiceGateway(IHttpClientFactory httpClientFactory, ILogger<MarsExplorationServiceGateway> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public HttpClient HttpClient => _httpClient ?? (_httpClient = _httpClientFactory.CreateClient(HttpClientDefaults.Name));

        public async Task<List<KeyValuePair<string, NavigationContext>>> GetRoverHistoryAsync()
        {
            _logger.LogInformation("Begining request for rover history");

            using (var response = await _httpClient.GetAsync(ExplorationEndpoint).ConfigureAwait(false))
            {
                var rawHistory = await response.Content.ReadAsAsync<List<KeyValuePair<string, NavigationContext>>>().ConfigureAwait(false);
                _logger.LogRawHistory(rawHistory);

                if (rawHistory == null || !rawHistory.Any())
                    return null;

                return rawHistory;
            }
        }

        public async Task<NavigationContext> InputRoverNavigationAsync(string roverNavigation)
        {
            _logger.LogInformation("Beginning request for rover navigation: {0}", roverNavigation);

            var content = new StringContent(roverNavigation);
            using(var response = await _httpClient.PostAsync(ExplorationEndpoint, content).ConfigureAwait(false))
            {
                var responseContent = await response.Content.ReadAsAsync<NavigationContext>().ConfigureAwait(false);

                return responseContent;
            }
        }
    }
}
