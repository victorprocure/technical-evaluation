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
            var originalCoordinates = rover.Coordinates;
            var originalHeading = rover.Heading;

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

            rover.Coordinates = originalCoordinates;
            rover.Heading = originalHeading;

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
            if (rover.Coordinates.X < Plateau.LowerLeftCoords.X || rover.Coordinates.X > Plateau.UpperRightCoords.X || rover.Coordinates.Y < Plateau.LowerLeftCoords.Y || rover.Coordinates.Y > Plateau.UpperRightCoords.Y)
            {
                throw new InvalidOperationException("Rover is not within the plateau");
            }
        }
    }
}