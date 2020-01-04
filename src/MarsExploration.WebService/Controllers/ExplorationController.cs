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
            // Double check string is actually not sent, fixes potential environmental issues
            if (string.IsNullOrEmpty(navigationString))
            {
                using (var reader = new StreamReader(Request.Body))
                {
                    navigationString = await reader.ReadToEndAsync();
                }
            }
            if (!_navigationStringValidator.IsValidNavigationString(navigationString))
                return BadRequest("Unable to validate navigation string");
            
            var historicalIO = _memoryCache.GetOrCreate<List<KeyValuePair<string, NavigationContext>>>(CacheKey, (_) => new List<KeyValuePair<string, NavigationContext>>());
            var cached = historicalIO.FirstOrDefault(kvp => kvp.Key.Equals(navigationString, StringComparison.OrdinalIgnoreCase));

            if(cached.Value != default)
                return Ok(cached.Value);

            var context = _navigationContextFactory.CreateFromNavigationString(navigationString);
            var navigationEngine = new NavigationEngine(context.Plateau);

            var roverLocations = ExecuteAllActionsOnNavigationEngine(navigationEngine, context);

            historicalIO.Add(new KeyValuePair<string, NavigationContext>(navigationString, context));
            return Ok(context);
        }

        [HttpGet]
        public IActionResult InputOutputHistory()
        {
            var historicalIO = _memoryCache.GetOrCreate(CacheKey, (_) => new List<KeyValuePair<string, NavigationContext>>());
            return Ok(historicalIO);
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
