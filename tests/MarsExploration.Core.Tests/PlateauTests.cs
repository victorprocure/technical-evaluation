using System.Drawing;
using NUnit.Framework;

namespace MarsExploration.Core.Tests
{
    public class PlateauTests
    {
        private readonly Point _defaultValidCoordinates = new Point(5, 5);

        [Test]
        public void GivenValidCoordinatesShouldCreatePlateau()
        {
            var coordinates = _defaultValidCoordinates;

            var plateau = new Plateau(coordinates);

            Assert.That(plateau.UpperRightCoords.X, Is.EqualTo(coordinates.X));
            Assert.That(plateau.UpperRightCoords.Y, Is.EqualTo(coordinates.Y));
        }

        [Test]
        public void GivenValidCoordinatesShouldSetLowerLeftCoordinates()
        {
            var coordinates = _defaultValidCoordinates;

            var plateau = new Plateau(coordinates);

            Assert.That(plateau.LowerLeftCoords.X, Is.EqualTo(0));
            Assert.That(plateau.LowerLeftCoords.Y, Is.EqualTo(0));
        }

        [TestCase(-1, -5)]
        [TestCase(-1, 0)]
        [TestCase(0, -4)]
        [TestCase(-3, 9)]
        [TestCase(9, -3)]
        public void GivenInvalidCoordinatesShouldThrowException(int x, int y)
        {
            var coordinates = new Point(x, y);

            Assert.That(() => new Plateau(coordinates), Throws.InvalidOperationException);
        }
    }
}