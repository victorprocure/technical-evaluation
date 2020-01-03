using System;

namespace MarsExploration.Core
{
    public enum Heading
    {
        North,

        East,

        South,

        West
    }

    internal static class HeadingExtensions
    {
        public static string ToShortString(this Heading heading)
        {
            switch(heading)
            {
                case Heading.North:
                    return "N";
                case Heading.East:
                    return "E";
                case Heading.South:
                    return "S";
                case Heading.West:
                    return "W";
            }

            throw new InvalidOperationException("Unable to conver heading to string");
        }
    }
}