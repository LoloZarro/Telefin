using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Telefin.Helper;

namespace Telefin.Web;

[Route("TelefinApi/SmokeTest")]
[ApiController]
public class SmokeTestController : ControllerBase
{
    private readonly ILogger<Plugin> _logger;
    private readonly TelegramSender _sender;

    public SmokeTestController(ILogger<Plugin> logger, TelegramSender sender)
    {
        _logger = logger;
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<string>> Get([FromQuery] string botToken, [FromQuery] string chatId, [FromQuery] string threadId)
    {
        string message = "[Telefin] Test message:\n🎉 Great news! Your configuration has been validated successfully! 🚀";

        bool result = await _sender.SendMessageAsync("Test", message, botToken, chatId, false, threadId).ConfigureAwait(false);

        if (result)
        {
            _logger.LogInformation("{PluginName}: Unable to send test notification!", typeof(Plugin).Name);
            return Ok("Success!");
        }
        else
        {
            _logger.LogInformation("{PluginName}: Test Notification sent successfully!", typeof(Plugin).Name);
            return BadRequest($"Unsuccessfull, check logs for details.");
        }
    }
}
