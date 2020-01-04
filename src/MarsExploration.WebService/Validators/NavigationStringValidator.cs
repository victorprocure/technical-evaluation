using System;
using System.Text.RegularExpressions;

namespace MarsExploration.WebService.Validators
{
    public class NavigationStringValidator : INavigationStringValidator
    {
        private const int MinimumLineCount = 3;
        private static readonly Regex _navigationLineValidator = new Regex(@"^[LRMlrm]+$", RegexOptions.Compiled);
        private static readonly Regex _plateauLineValidator = new Regex(@"\d+\s\d+", RegexOptions.Compiled);
        private static readonly Regex _roverLineValidator = new Regex(@"\d+\s\d+\s[nNeEsSwW]", RegexOptions.Compiled);

        public bool IsValidNavigationString(string navigationString)
        {
            if (string.IsNullOrEmpty(navigationString))
                return false;

            var stringLines = navigationString.Split(Environment.NewLine);
            if (stringLines.Length < MinimumLineCount)
            {
                stringLines = navigationString.Split("\n"); // handle different hosting environments
                if (stringLines.Length < MinimumLineCount)
                    return false;
            }

            // Validate odd number of lines, ensuring all rovers have actions
            if (stringLines.Length % 2 == 0)
                return false;

            var plateauUpperRight = stringLines[0].Trim();
            if (!IsPlateauLineValid(plateauUpperRight))
                return false;

            for (var i = 1; i < stringLines.Length; i++)
            {
                if (i % 2 == 0) // Even line indexes are navigation actions
                {
                    if (!IsNavigationActionLineValid(stringLines[i].Trim()))
                        return false;
                }
                else
                {
                    if (!IsRoverLineValid(stringLines[i].Trim()))
                        return false;
                }
            }

            return true;
        }

        private static bool IsNavigationActionLineValid(string navigationLine)
            => _navigationLineValidator.IsMatch(navigationLine);

        private static bool IsPlateauLineValid(string plateauLine)
            => _plateauLineValidator.IsMatch(plateauLine);

        private static bool IsRoverLineValid(string roverLine)
            => _roverLineValidator.IsMatch(roverLine);
    }
}
