using System;
using System.Collections.Generic;
using System.Drawing;
using MarsExploration.Core;

namespace MarsExploration.WebService.Factories
{
    internal class NavigationContextFactory : INavigationContextFactory
    {
        public NavigationContext CreateFromNavigationString(string navigationString)
        {
            var stringLines = navigationString.Split(Environment.NewLine);
            if (stringLines.Length == 1)
            {
                stringLines = navigationString.Split("\n");
                if (stringLines.Length == 1)
                    throw new InvalidOperationException("Unable to parse navigation string");
            }

            var plateau = CreatePlateauFromLine(stringLines[0].Trim());
            var navigationActions = new List<IEnumerable<NavigationAction>>();
            var rovers = new List<Rover>();

            for (var i = 1; i < stringLines.Length; i++)
            {
                if (i % 2 == 0) // navigation actions are even lines
                    navigationActions.Add(CreateNavigationActionsFromLine(stringLines[i].Trim()));
                else
                    rovers.Add(CreateRoversFromLine(stringLines[i].Trim()));
            }

            return new NavigationContext(plateau, rovers, navigationActions);
        }

        private Rover CreateRoversFromLine(string roverLine)
        {
            var roverDetails = roverLine.Split(" ");
            if (!int.TryParse(roverDetails[0], out var x) || !int.TryParse(roverDetails[1], out var y))
                throw new InvalidOperationException("Unable to parse rover line coordinates are invalid");

            var heading = GetHeadingFromChar(roverDetails[2][0]);

            return new Rover(new Point(x, y), heading);
        }

        private Heading GetHeadingFromChar(char h)
        {
            switch (h)
            {
                case 'N':
                case 'n':
                    return Heading.North;
                case 'E':
                case 'e':
                    return Heading.East;
                case 'S':
                case 's':
                    return Heading.South;
                case 'W':
                case 'w':
                    return Heading.West;
            }

            throw new InvalidOperationException($"{h} Cannot be converted to heading");
        }

        private IEnumerable<NavigationAction> CreateNavigationActionsFromLine(string navigationActionLine)
        {
            foreach (char c in navigationActionLine)
            {
                switch (c)
                {
                    case 'L':
                    case 'l':
                        yield return NavigationAction.L;
                        break;
                    case 'R':
                    case 'r':
                        yield return NavigationAction.R;
                        break;
                    case 'M':
                    case 'm':
                        yield return NavigationAction.M;
                        break;
                }
            }
        }

        private static Plateau CreatePlateauFromLine(string plateauCoordinatesLine)
        {
            var plateauCoordinateStrings = plateauCoordinatesLine.Split(" ");

            if (int.TryParse(plateauCoordinateStrings[0], out var x) && int.TryParse(plateauCoordinateStrings[1], out var y))
                return new Plateau(new Point(x, y));

            throw new InvalidOperationException("Unable to parse plateau line");
        }
    }
}
