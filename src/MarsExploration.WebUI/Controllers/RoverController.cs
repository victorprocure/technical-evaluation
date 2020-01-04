using System.IO;
using System.Threading.Tasks;
using MarsExploration.WebUI.Gateways;
using MarsExploration.WebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace MarsExploration.WebUI.Controllers
{
    public class RoverController: Controller
    {
        private readonly IMarsExplorationServiceGateway _serviceGateway;

        public RoverController(IMarsExplorationServiceGateway serviceGateway)
        {
            _serviceGateway = serviceGateway;
        }

        [HttpPost]
        public async Task<IActionResult> Index(string navigationString)
        {
            // Double check string is actually not sent, fixes potential environmental issues
            if (string.IsNullOrEmpty(navigationString))
            {
                using (var reader = new StreamReader(Request.Body))
                {
                    navigationString = await reader.ReadToEndAsync();
                }
            }
            var response = await _serviceGateway.InputRoverNavigationAsync(navigationString);

            var model = new RoverGridViewModel{NavigationContext = response};

            return PartialView("_RoverGrid", model);
        }
    }
}
