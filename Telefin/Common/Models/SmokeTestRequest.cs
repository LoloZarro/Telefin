using System.Collections.Generic;

namespace Telefin.Common.Models
{
    public class SmokeTestRequest
    {
        public string BotToken { get; set; } = string.Empty;

        public List<ConfiguredChat> ConfiguredChats { get; set; } = new();
    }
}
