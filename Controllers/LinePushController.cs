using Line.Messaging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace childs_notification.Controllers
{
    [ApiController]
    public class LinePushController : ControllerBase
    {
        private static readonly TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
        private readonly LineMessagingClient lineMessagingClient;
        private readonly IConfiguration configuration;
        private readonly ILogger<LinePushController> logger;
        private readonly IHostingEnvironment env;

        public LinePushController(
            IConfiguration configuration,
            ILogger<LinePushController> logger,
            IHostingEnvironment env)
        {
            this.configuration = configuration;
            this.logger = logger;
            this.env = env;

            lineMessagingClient = new LineMessagingClient(configuration["Line:ChannelAccessToken"]);
        }

        [HttpGet]
        [Route("push")]
        public async Task<IActionResult> Get()
        {
            var message = GetMessage();
            logger.LogInformation("message: {message}", message);
            if (!string.IsNullOrWhiteSpace(message))
            {
                await lineMessagingClient.PushMessageAsync(configuration["RoomId"], message);
            }
            return Content(message);
        }

#if false
        private string GetUserName(string id)
        {
            var envUsers = Environment.GetEnvironmentVariable("Users");
            var convertedUsers = envUsers == null ? null : JsonConvert.DeserializeObject<IEnumerable<LineUser>>(envUsers);
            var users = convertedUsers ?? appsettings.LineSettings.Users;
            return users.FirstOrDefault(u => u.Id == id)?.Name;
        }
#endif

        private string GetMessage()
        {
            var now = TimeZoneInfo.ConvertTime(DateTime.UtcNow, tzi);
            var message = string.Empty;
            if (env.IsDevelopment())
            {
                message = $"さんぷるめっせーじです - {now.ToShortTimeString()}";
            }
            else if (7 <= now.Hour && now.Hour < 10)
            {
                message = "保育園に送りました";
            }
            else if (17 <= now.Hour && now.Hour < 20)
            {
                message = "保育園に迎えに来ました";
            }
            return message;
        }
    }
}
