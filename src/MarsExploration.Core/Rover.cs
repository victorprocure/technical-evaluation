using System.Drawing;

namespace MarsExploration.Core
{
    public class Rover
    {
        public Point Coordinates { get; private set; }
        public Heading Heading { get; private set; }

        public Rover(Point initialCoordinates, Heading initialHeading)
        {
            Coordinates = initialCoordinates;
            Heading = initialHeading;
        }

        public void Turn(TurnDirection turnDirection)
        {
            int headingCalculation = ((int)Heading) + ((int)turnDirection);
            switch (headingCalculation)
            {
                case -1:
                    headingCalculation = 3; // will convert to west
                    break;
                case 4:
                    headingCalculation = 0; // will convert to north
                    break;
            }

            Heading = (Heading)headingCalculation;
        }

        public void Move()
        {
            switch (Heading)
            {
                case Heading.North:
                    Coordinates = new Point(Coordinates.X, Coordinates.Y + 1);
                    break;
                case Heading.South:
                    Coordinates = new Point(Coordinates.X, Coordinates.Y - 1);
                    break;
                case Heading.East:
                    Coordinates = new Point(Coordinates.X + 1, Coordinates.Y);
                    break;
                case Heading.West:
                    Coordinates = new Point(Coordinates.X - 1, Coordinates.Y);
                    break;
            }
        }
    }
}