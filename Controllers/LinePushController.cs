using Line.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using childs_notification.Models;
using System;
using Microsoft.Extensions.Logging;

namespace childs_notification.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class LinePushController : Controller
    {
        private static LineMessagingClient lineMessagingClient;
        private readonly ILogger logger;
        AppSettings appsettings;
        public LinePushController(IOptions<AppSettings> options, ILogger<LinePushController> _logger)
        {
            appsettings = options.Value;
            var channelAccessToken = Environment.GetEnvironmentVariable("ChannelAccessToken") ?? appsettings.LineSettings.ChannelAccessToken;
            lineMessagingClient = new LineMessagingClient(channelAccessToken);
            logger = _logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var to = Environment.GetEnvironmentVariable("RoomId") ?? appsettings.LineSettings.RoomId;
            var message = GetMessage();
            if (!string.IsNullOrWhiteSpace(message))
            {
                await lineMessagingClient.PushMessageAsync(to, message);
            }
            return new OkResult();
        }

        private string GetMessage()
        {
            var tzi = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
            var now = TimeZoneInfo.ConvertTime(DateTime.Now.ToUniversalTime(), tzi);
            if (7 <= now.Hour && now.Hour < 10)
            {
                return "保育園に送りました";
            }
            else if (17 <= now.Hour && now.Hour < 20)
            {
                return "保育園に迎えに来ました";
            }
            else
            {
                return null;
            }
        }
    }
}
