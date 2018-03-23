using Line.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using childs_notification.Models;
using System;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.AspNetCore.Hosting;

namespace childs_notification.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class LinePushController : Controller
    {
        private static LineMessagingClient lineMessagingClient;
        private readonly ILogger logger;
        AppSettings appsettings;
        IHostingEnvironment env;

        public LinePushController(
            IOptions<AppSettings> options,
            ILogger<LinePushController> logger,
            IHostingEnvironment env)
        {
            appsettings = options.Value;
            this.logger = logger;
            this.env = env;

            var channelAccessToken = Environment.GetEnvironmentVariable("ChannelAccessToken") ?? appsettings.LineSettings.ChannelAccessToken;
            lineMessagingClient = new LineMessagingClient(channelAccessToken);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new { Error = $"{nameof(id)}が指定されていません。" });
            }

            var message = GetMessage(GetUserName(id));
            if (!string.IsNullOrWhiteSpace(message))
            {
                var to = Environment.GetEnvironmentVariable("RoomId") ?? appsettings.LineSettings.RoomId;
                await lineMessagingClient.PushMessageAsync(to, message);
            }
            return Ok(new { Message = message });
        }

        private string GetUserName(string id)
        {
            return appsettings.LineSettings.Users.FirstOrDefault(u => u.Id == id)?.Name;
        }

        private string GetMessage(string name)
        {
            var tzi = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
            var now = TimeZoneInfo.ConvertTime(DateTime.Now.ToUniversalTime(), tzi);
            var message = string.Empty;

            if (7 <= now.Hour && now.Hour < 10)
            {
                message = "保育園に送りました";
            }
            else if (17 <= now.Hour && now.Hour < 20)
            {
                message = "保育園に迎えに来ました";
            }
            else if (env.IsDevelopment())
            {
                message = $"さんぷるめっせーじです - {DateTime.Now.ToShortTimeString()}";
            }

            if (!string.IsNullOrWhiteSpace(name)
                && !string.IsNullOrWhiteSpace(message))
            {
                message = $"({name}) {message}";
            }
            return message;
        }
    }
}
