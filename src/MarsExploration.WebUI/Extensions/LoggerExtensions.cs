using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using MarsExploration.Core;

namespace MarsExploration.WebUI.Extensions
{
    /// <summary>
    /// Adding some semantic logging just for the service return
    /// </summary>
    internal static class LoggerExtensions
    {
        private static readonly Action<ILogger, List<KeyValuePair<string, NavigationContext>>, Exception> _logRawHistory = LoggerMessage.Define<List<KeyValuePair<string, NavigationContext>>>(
            logLevel: LogLevel.Debug,
            eventId: new EventId(1, nameof(LogRawHistory)),
            formatString: "Raw history from service {rawHistory}"
        );

        public static void LogRawHistory(this ILogger logger, List<KeyValuePair<string, NavigationContext>> rawHistory)
        {
            _logRawHistory(logger, rawHistory, null);
        }
    }
}
