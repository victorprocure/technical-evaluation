using System.Drawing;
using NUnit.Framework;
using MarsExploration.Core;

namespace MarsExploration.Core.Tests
{
    public class RoverTests
    {
        [Test]
        public void GivenNewRoverShouldHaveInitialCoordinatesAndHeading()
        {
            var initialCoordinates = new Point(1, 2);
            var initialHeading = Heading.North;

            var rover = new Rover(initialCoordinates, initialHeading);

            Assert.That(rover.CurrentCoordinates.X, Is.EqualTo(initialCoordinates.X));
            Assert.That(rover.CurrentCoordinates.Y, Is.EqualTo(initialCoordinates.Y));
            Assert.That(rover.CurrentHeading, Is.EqualTo(initialHeading));
        }

        [TestCase(TurnDirection.Left, Heading.East, ExpectedResult = Heading.North)]
        [TestCase(TurnDirection.Right, Heading.East, ExpectedResult = Heading.South)]
        [TestCase(TurnDirection.Left, Heading.West, ExpectedResult = Heading.South)]
        [TestCase(TurnDirection.Right, Heading.West, ExpectedResult = Heading.North)]
        [TestCase(TurnDirection.Left, Heading.North, ExpectedResult = Heading.West)]
        [TestCase(TurnDirection.Right, Heading.North, ExpectedResult = Heading.East)]
        [TestCase(TurnDirection.Left, Heading.South, ExpectedResult = Heading.East)]
        [TestCase(TurnDirection.Right, Heading.South, ExpectedResult = Heading.West)]
        public Heading GivenValidTurnDirectionRoverShouldChangeHeading(TurnDirection turnDirection, Heading initialHeading)
        {
            var initialCoordinates = new Point(0, 0);
            var rover = new Rover(initialCoordinates, initialHeading);

            rover.Turn(turnDirection);

            return rover.CurrentHeading;
        }

        [TestCase(1, 2, Heading.North, 3)]
        [TestCase(3, 3, Heading.East, 4)]
        [TestCase(5, 7, Heading.West, 4)]
        [TestCase(3, 6, Heading.South, 5)]
        public void GivenRoverShouldBeAbleToMove(int initialX, int initialY, Heading initialHeading, int expectedCoordinateChange)
        {
            var initialCoordinates = new Point(initialX, initialY);
            var rover = new Rover(initialCoordinates, initialHeading);

            rover.Move();

            if (initialHeading == Heading.North || initialHeading == Heading.South)
            {
                Assert.That(rover.CurrentCoordinates.Y, Is.EqualTo(expectedCoordinateChange));
                Assert.That(rover.CurrentCoordinates.X, Is.EqualTo(initialX));
            }
            else
            {
                Assert.That(rover.CurrentCoordinates.Y, Is.EqualTo(initialY));
                Assert.That(rover.CurrentCoordinates.X, Is.EqualTo(expectedCoordinateChange));
            }
        }
    }
}
