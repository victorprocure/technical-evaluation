using System.Collections.Generic;
using MarsExploration.Core;

namespace MarsExploration.WebUI.Models
{
    public class HistoryViewModel
    {
        public List<KeyValuePair<string, NavigationContext>> HistoricalData { get; set; }
    }
}
