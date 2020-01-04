using System.Drawing;

namespace MarsExploration.Core
{
    public class Rover
    {
        public Point CurrentCoordinates { get; internal set; }
        public Heading CurrentHeading { get; internal set; }

        public Point InitialCoordinates { get; internal set; }
        public Heading InitialHeading { get; internal set; }

        public Rover(Point initialCoordinates, Heading initialHeading)
        {
            InitialCoordinates = CurrentCoordinates = initialCoordinates;
            InitialHeading = CurrentHeading = initialHeading;
        }

        public void Turn(TurnDirection turnDirection)
        {
            int headingCalculation = ((int)CurrentHeading) + ((int)turnDirection);
            switch (headingCalculation)
            {
                case -1:
                    headingCalculation = 3; // will convert to west
                    break;
                case 4:
                    headingCalculation = 0; // will convert to north
                    break;
            }

            CurrentHeading = (Heading)headingCalculation;
        }

        public void Move()
        {
            switch (CurrentHeading)
            {
                case Heading.North:
                    CurrentCoordinates = new Point(CurrentCoordinates.X, CurrentCoordinates.Y + 1);
                    break;
                case Heading.South:
                    CurrentCoordinates = new Point(CurrentCoordinates.X, CurrentCoordinates.Y - 1);
                    break;
                case Heading.East:
                    CurrentCoordinates = new Point(CurrentCoordinates.X + 1, CurrentCoordinates.Y);
                    break;
                case Heading.West:
                    CurrentCoordinates = new Point(CurrentCoordinates.X - 1, CurrentCoordinates.Y);
                    break;
            }
        }

        public override string ToString()
        {
            return $"{CurrentCoordinates.X} {CurrentCoordinates.Y} {CurrentHeading.ToShortString()}";
        }
    }
}
