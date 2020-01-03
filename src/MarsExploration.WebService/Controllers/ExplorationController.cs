using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MarsExploration.Core;
using MarsExploration.WebService.Factories;
using MarsExploration.WebService.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace MarsExploration.WebService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExplorationController : ControllerBase
    {
        private const string CacheKey = "exploration-cache";

        private readonly ILogger<ExplorationController> _logger;
        private readonly INavigationStringValidator _navigationStringValidator;
        private readonly INavigationContextFactory _navigationContextFactory;
        private readonly IMemoryCache _memoryCache;

        public ExplorationController(INavigationStringValidator navigationStringValidator, INavigationContextFactory navigationContextFactory, IMemoryCache cache, ILogger<ExplorationController> logger)
        {
            _navigationStringValidator = navigationStringValidator;
            _navigationContextFactory = navigationContextFactory;
            _memoryCache = cache;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> InputNavigationString(string navigationString)
        {
            if (string.IsNullOrEmpty(navigationString))
            {
                using (var reader = new StreamReader(Request.Body))
                {
                    navigationString = await reader.ReadToEndAsync();
                }
            }
            if (!_navigationStringValidator.IsValidNavigationString(navigationString))
                return BadRequest("Unable to validate navigation string");
            
            var historicalIO = _memoryCache.GetOrCreate<List<KeyValuePair<string, string>>>(CacheKey, (_) => new List<KeyValuePair<string, string>>());
            var cached = historicalIO.FirstOrDefault(kvp => kvp.Key.Equals(navigationString, StringComparison.OrdinalIgnoreCase));

            if(cached.Value != default)
                return Ok(cached.Value);

            var context = _navigationContextFactory.CreateFromNavigationString(navigationString);
            var navigationEngine = new NavigationEngine(context.Plateau);

            var roverLocations = ExecuteAllActionsOnNavigationEngine(navigationEngine, context);
            var result = string.Join(Environment.NewLine, roverLocations);

            
            historicalIO.Add(new KeyValuePair<string, string>(navigationString, result));
            return Ok(result);
        }

        [HttpGet]
        public IActionResult InputOutputHistory()
        {
            var historicalIO = _memoryCache.GetOrCreate(CacheKey, (_) => new List<KeyValuePair<string, string>>());
            return Ok(historicalIO.Select(kvp => $"Input: {kvp.Key}{Environment.NewLine}Output: {kvp.Value}"));
        }

        private IEnumerable<string> ExecuteAllActionsOnNavigationEngine(NavigationEngine engine, NavigationContext context)
        {
            for (var i = 0; i < context.Rovers.Count(); i++)
            {
                var rover = context.Rovers[i];
                engine.Guide(rover, context.NavigationActions[i].ToArray());

                yield return rover.ToString();
            }
        }
    }
}