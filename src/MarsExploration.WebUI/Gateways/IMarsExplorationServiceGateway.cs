using System.Collections.Generic;
using System.Threading.Tasks;
using MarsExploration.Core;

namespace MarsExploration.WebUI.Gateways
{
    public interface IMarsExplorationServiceGateway
    {
         Task<List<KeyValuePair<string, NavigationContext>>> GetRoverHistoryAsync();

         Task<NavigationContext> InputRoverNavigationAsync(string roverNavigation);
    }
}
