using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telefin.Helper;

namespace Telefin.Telegram.Configuration;

[Route("TelefinApi/TestNotifier")]
[ApiController]
public class TestNotifier : ControllerBase
{
    private readonly TelegramSender _sender;

    public TestNotifier(TelegramSender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<ActionResult<string>> Get([FromQuery] string botToken, [FromQuery] string chatId, [FromQuery] string threadId)
    {
        string message = "[Telefin] Test message: \n 🎉 Your configuration is correct ! 🥳";

        bool result = await _sender.SendMessageAsync("Test", message, botToken, chatId, false, threadId).ConfigureAwait(false);

        if (result)
        {
            return Ok("Message sent successfully");
        }
        else
        {
            return BadRequest($"Message could not be sent, please check logs for more details.");
        }
    }
}
