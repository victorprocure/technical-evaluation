using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MarsExploration.Core;
using MarsExploration.WebUI.Configurations;
using MarsExploration.WebUI.Converter;
using MarsExploration.WebUI.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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

            using (var response = await HttpClient.GetAsync(ExplorationEndpoint).ConfigureAwait(false))
            {
                var resposneString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var rawHistory = JsonConvert.DeserializeObject<List<KeyValuePair<string, NavigationContext>>>(resposneString, new PointConverter());
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
            using (var response = await HttpClient.PostAsync(ExplorationEndpoint, content).ConfigureAwait(false))
            {
                var resposneString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var responseContent = JsonConvert.DeserializeObject<NavigationContext>(resposneString, new PointConverter());

                return responseContent;
            }
        }
    }
}
