using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Telefin.Common.Models;
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

    [HttpPost("SmokeTest")]
    public async Task<ActionResult<string>> SmokeTest([FromBody] SmokeTestRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.BotToken))
        {
            return BadRequest("Missing botToken.");
        }

        string message =
            "[Telefin] Test message:\n\n" +
            "🎉 Great news! Your configuration has been validated successfully! 🚀";

        var results = new List<bool>();

        foreach (var chat in request.ConfiguredChats ?? new List<ConfiguredChat>())
        {
            var chatId = chat.ChatId?.Trim();
            if (string.IsNullOrWhiteSpace(chatId))
            {
                continue;
            }

            var threadId = string.IsNullOrWhiteSpace(chat.ThreadId) ? null : chat.ThreadId.Trim();

            results.Add(await _sender.SendMessageAsync(
                    "Test",
                    message,
                    request.BotToken,
                    chatId,
                    false,
                    threadId ?? string.Empty)
                .ConfigureAwait(false));
        }

        if (results.Count > 0 && results.All(r => r))
        {
            _logger.LogInformation("{PluginName}: Test notification(s) sent successfully!", Plugin.PluginName);
            return Ok("Success!");
        }

        _logger.LogError("{PluginName}: Unable to send test to one or more chats.", Plugin.PluginName);
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
