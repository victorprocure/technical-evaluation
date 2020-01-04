using System;
using System.Drawing;
using NUnit.Framework;

namespace MarsExploration.Core.Tests
{
    public class NavigationEngineTests
    {
        [Test]
        public void GivenValidInputShouldCreateNavigationEngine()
        {
            var plateau = new Plateau(new Point(5, 5));
            var rover = new Rover(new Point(1, 2), Heading.North);

            var navigationEngine = new NavigationEngine(plateau, rover);

            Assert.That(navigationEngine.RoverActionsCache.Count, Is.EqualTo(1));
            Assert.That(navigationEngine.Plateau, Is.Not.Null);
        }

        [Test]
        public void GivenRoverOutsidePlateauShouldThrow()
        {
            var plateau = new Plateau(new Point(5, 5));
            var rover = new Rover(new Point(6, 2), Heading.North);

            Assert.That(() => new NavigationEngine(plateau, rover), Throws.InvalidOperationException);
        }

        [TestCase(1, 2, Heading.North, 1, 3, Heading.North, NavigationAction.L, NavigationAction.M, NavigationAction.L, NavigationAction.M, NavigationAction.L, NavigationAction.M, NavigationAction.L, NavigationAction.M, NavigationAction.M)]
        [TestCase(3, 3, Heading.East, 5, 1, Heading.East, NavigationAction.M, NavigationAction.M, NavigationAction.R, NavigationAction.M, NavigationAction.M, NavigationAction.R, NavigationAction.M, NavigationAction.R, NavigationAction.R, NavigationAction.M)]
        public void GivenValidActionsShouldGuideRover(int roverX, int roverY, Heading roverHeading, int expectedX, int expectedY, Heading expectedHeading, params NavigationAction[] navigationActions)
        {
            var plateau = new Plateau(new Point(5, 5));
            var rover = new Rover(new Point(roverX, roverY), roverHeading);
            var navigationEngine = new NavigationEngine(plateau, rover);

            navigationEngine.Guide(rover, navigationActions);

            Assert.That(rover.CurrentCoordinates.X, Is.EqualTo(expectedX));
            Assert.That(rover.CurrentCoordinates.Y, Is.EqualTo(expectedY));
            Assert.That(rover.CurrentHeading, Is.EqualTo(expectedHeading));
        }

        [Test]
        public void GivenNavigationActionWouldRemoveRoverFromPlateauShouldThrow()
        {
            var plateau = new Plateau(new Point(5, 5));
            var rover = new Rover(new Point(5, 4), Heading.East);
            var navigationEngine = new NavigationEngine(plateau, rover);

            Assert.That(() => navigationEngine.Guide(rover, NavigationAction.M), Throws.InvalidOperationException);
        }

        [Test]
        public void GivenNavigationActionWouldRemoveRoverFromPlateauShouldResetRoversPosition()
        {
            var plateau = new Plateau(new Point(5, 5));
            var initialX = 5;
            var initialY = 4;
            var initialHeading = Heading.East;
            var rover = new Rover(new Point(initialX, initialY), initialHeading);
            var navigationEngine = new NavigationEngine(plateau, rover);

            try
            {
                navigationEngine.Guide(rover, NavigationAction.M);
            }
            catch (InvalidOperationException)
            {
                // Do nothing
            }

            Assert.That(rover.CurrentCoordinates.X, Is.EqualTo(initialX));
            Assert.That(rover.CurrentCoordinates.Y, Is.EqualTo(initialY));
            Assert.That(rover.CurrentHeading, Is.EqualTo(initialHeading));
        }

        [Test]
        public void GivenNavigationActionWouldRemoveRoverFromPlateauShouldNotAddActionToCache()
        {
            var plateau = new Plateau(new Point(5, 5));
            var initialX = 5;
            var initialY = 4;
            var initialHeading = Heading.East;
            var rover = new Rover(new Point(initialX, initialY), initialHeading);
            var navigationEngine = new NavigationEngine(plateau, rover);

            try
            {
                navigationEngine.Guide(rover, NavigationAction.M);
            }
            catch (InvalidOperationException)
            {
                // Do nothing
            }

            Assert.That(navigationEngine.RoverActionsCache[rover], Is.Empty);
        }

        [Test]
        public void GivenNavigationActionWouldMoveRoverShouldAddActionToCache()
        {
            var plateau = new Plateau(new Point(7, 7));
            var initialX = 5;
            var initialY = 4;
            var initialHeading = Heading.East;
            var rover = new Rover(new Point(initialX, initialY), initialHeading);
            var navigationEngine = new NavigationEngine(plateau, rover);

            navigationEngine.Guide(rover, NavigationAction.M);


            Assert.That(navigationEngine.RoverActionsCache[rover], Is.Not.Empty);
        }
    }
}
