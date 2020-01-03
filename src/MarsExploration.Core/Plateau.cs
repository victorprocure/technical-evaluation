using System;
using System.Drawing;

namespace MarsExploration.Core
{
    public class Plateau
    {
        public Point UpperRightCoords { get; }
        public Point LowerLeftCoords { get; } = new Point(0, 0);

        public Plateau(Point upperRightCoords)
        {
            ValidateCoordinates(upperRightCoords, LowerLeftCoords);
            
            UpperRightCoords = upperRightCoords;
        }

        private static void ValidateCoordinates(Point upperRightCoords, Point lowerLeftCoords)
        {
            if (upperRightCoords.X > lowerLeftCoords.X && upperRightCoords.Y > lowerLeftCoords.Y)
                return;

            throw new InvalidOperationException("Upper right coordinates cannot be less than lower right coordinates");
        }
    }
}