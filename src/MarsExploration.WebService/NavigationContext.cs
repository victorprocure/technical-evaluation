using System.Collections.Generic;
using MarsExploration.Core;

namespace MarsExploration.WebService
{
    public class NavigationContext
    {
        public IList<Rover> Rovers { get; }
        public IList<IEnumerable<NavigationAction>> NavigationActions { get; }
        public Plateau Plateau { get; }

        public NavigationContext(Plateau plateau, IList<Rover> rovers, IList<IEnumerable<NavigationAction>> navigationActions)
        {
            Rovers = rovers;
            NavigationActions = navigationActions;
            Plateau = plateau;
        }
    }
}