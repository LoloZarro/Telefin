using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Telefin.Configuration;
using Telefin.Helper;

namespace Telefin.Web;

[Route("TelefinApi")]
[ApiController]
public class TelefinApiController : ControllerBase
{
    private readonly ILogger<Plugin> _logger;
    private readonly TelegramSender _sender;

    public TelefinApiController(ILogger<Plugin> logger, TelegramSender sender)
    {
        _logger = logger;
        _sender = sender;
    }

    [HttpGet("SmokeTest")]
    public async Task<ActionResult<string>> SmokeTest(
        [FromQuery] string botToken,
        [FromQuery] string chatId,
        [FromQuery] string threadId)
    {
        string message =
            "[Telefin] Test message:\n\n" +
            "🎉 Great news! Your configuration has been validated successfully! 🚀";

        bool result = await _sender.SendMessageAsync(
            "Test",
            message,
            botToken,
            chatId,
            false,
            threadId)
            .ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("{PluginName}: Test notification sent successfully!", typeof(Plugin).Name);
            return Ok("Success!");
        }

        _logger.LogError("{PluginName}: Unable to send test notification!",typeof(Plugin).Name);
        return BadRequest("Unsuccessful, check logs for details.");
    }

    [HttpGet("DefaultMessages")]
    public ActionResult<IDictionary<string, string>> GetDefaultMessages()
    {
        var messages = typeof(DefaultMessages)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(p => p.PropertyType == typeof(string))
            .ToDictionary(
                p => p.Name,
                p => (string)(p.GetValue(null) ?? string.Empty));

        return Ok(messages);
    }
}
