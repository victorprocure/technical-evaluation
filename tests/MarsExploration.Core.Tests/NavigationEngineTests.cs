using System.Drawing;
using System.Linq;
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

            Assert.That(navigationEngine.Rovers.Count(), Is.EqualTo(1));
            Assert.That(navigationEngine.Plateau, Is.Not.Null);
        }

        [Test]
        public void GivenRoverOutsidePlateauShouldThrow()
        {
            var plateau = new Plateau(new Point(5, 5));
            var rover = new Rover(new Point(6, 2), Heading.North);

            Assert.That(() => new NavigationEngine(plateau, rover), Throws.InvalidOperationException);
        }
    }
}