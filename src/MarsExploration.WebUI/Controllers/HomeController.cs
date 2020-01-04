using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MarsExploration.WebUI.Models;
using MarsExploration.WebUI.Gateways;

namespace MarsExploration.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMarsExplorationServiceGateway _marsServiceGateway;

        public HomeController(IMarsExplorationServiceGateway marsServiceGateway, ILogger<HomeController> logger)
        {
            _marsServiceGateway = marsServiceGateway;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> History()
        {
            var historicalData = await _marsServiceGateway.GetRoverHistoryAsync();
            var viewModel = new HistoryViewModel
            {
                HistoricalData = historicalData
            };

            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
