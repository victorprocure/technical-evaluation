using System;
using System.Collections.Generic;

namespace MarsExploration.Core
{
    public class NavigationEngine
    {
        public Plateau Plateau { get; }
        public Dictionary<Rover, List<NavigationAction>> RoverActionsCache { get; } = new Dictionary<Rover, List<NavigationAction>>();

        public NavigationEngine(Plateau plateau, params Rover[] rovers) : this(plateau)
        {
            AddRoversToCache(rovers);
        }

        public NavigationEngine(Plateau plateau)
        {
            Plateau = plateau;
        }

        public void Guide(Rover rover, params NavigationAction[] navigationActions)
        {
            if (!RoverActionsCache.ContainsKey(rover))
                AddRoversToCache(rover);

            foreach (var navigationAction in navigationActions)
            {
                if (!HandleNavigationAction(rover, navigationAction))
                    throw new InvalidOperationException("Rover could not execute navigation action");
            }
        }

        private bool HandleNavigationAction(Rover rover, NavigationAction navigationAction)
        {
            var originalCoordinates = rover.CurrentCoordinates;
            var originalHeading = rover.CurrentHeading;

            switch (navigationAction)
            {
                case NavigationAction.L:
                    rover.Turn(TurnDirection.Left);
                    break;
                case NavigationAction.R:
                    rover.Turn(TurnDirection.Right);
                    break;
                case NavigationAction.M:
                    rover.Move();
                    break;
            }

            if (TryValidateRoverOnPlateau(rover))
            {
                RoverActionsCache[rover].Add(navigationAction);
                return true;
            }

            rover.CurrentCoordinates = originalCoordinates;
            rover.CurrentHeading = originalHeading;

            return false;
        }

        private void AddRoversToCache(params Rover[] rovers)
        {
            foreach (var rover in rovers)
            {
                if (RoverActionsCache.ContainsKey(rover))
                    return;

                ValidateRoverOnPlateau(rover);
                RoverActionsCache.Add(rover, new List<NavigationAction>());
            }
        }

        private bool TryValidateRoverOnPlateau(Rover rover)
        {
            try
            {
                ValidateRoverOnPlateau(rover);
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        private void ValidateRoverOnPlateau(Rover rover)
        {
            if (rover.CurrentCoordinates.X < Plateau.LowerLeftCoords.X || rover.CurrentCoordinates.X > Plateau.UpperRightCoords.X || rover.CurrentCoordinates.Y < Plateau.LowerLeftCoords.Y || rover.CurrentCoordinates.Y > Plateau.UpperRightCoords.Y)
            {
                throw new InvalidOperationException("Rover is not within the plateau");
            }
        }
    }
}
