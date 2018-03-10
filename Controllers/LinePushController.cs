using Line.Messaging;
using Line.Messaging.Webhooks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using childs_notification.CloudStorage;
using childs_notification.Models;
using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

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
            var messages = new List<ISendMessage> { new TextMessage(GetMessage()) };
            await lineMessagingClient.PushMessageAsync(to, messages);
            return new OkResult();
        }

        private string GetMessage()
        {
            var tzi = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
            var now = TimeZoneInfo.ConvertTime(DateTime.Now.ToUniversalTime(), tzi);
            if (0 <= now.Hour && now.Hour < 12)
            {
                return "保育園に送りました";
            }
            else
            {
                return "保育園に迎えに来ました";
            }
        }
    }
}
