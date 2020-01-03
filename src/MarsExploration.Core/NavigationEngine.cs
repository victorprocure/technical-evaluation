using System;
using System.Collections.Generic;

namespace MarsExploration.Core
{
    public class NavigationEngine
    {
        public Plateau Plateau { get; }
        public Rover[] Rovers { get; }

        public NavigationEngine(Plateau plateau, params Rover[] rovers)
        {
            Plateau = plateau;
            
            ValidateRoversOnPlateau(rovers);
            Rovers = rovers;
        }

        private void ValidateRoversOnPlateau(Rover[] rovers)
        {
            foreach (var rover in rovers)
            {
                if (rover.Coordinates.X < Plateau.LowerLeftCoords.X || rover.Coordinates.X > Plateau.UpperRightCoords.X || rover.Coordinates.Y < Plateau.LowerLeftCoords.Y || rover.Coordinates.Y > Plateau.UpperRightCoords.Y)
                {
                    throw new InvalidOperationException("Rover is not within the plateau");
                }
            }
        }
    }
}