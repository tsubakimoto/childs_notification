using childs_notification.CloudStorage;
using childs_notification.Models;
using Line.Messaging;
using Line.Messaging.Webhooks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace childs_notification.Controllers
{
    [ApiController]
    public class LineBotController : ControllerBase
    {
        private readonly LineMessagingClient line;
        private readonly IConfiguration configuration;
        private readonly ILogger<LineBotController> logger;

        public LineBotController(IConfiguration configuration, ILogger<LineBotController> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
            line = new LineMessagingClient(configuration["Line:ChannelAccessToken"]);
        }

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        [HttpPost]
        [Route("api/messages")]
        public async Task<IActionResult> Post(HttpRequestMessage request)
        {
            var events = await request.GetWebhookEventsAsync(configuration["Line:ChannelSecret"]);
            var connectionString = configuration["StorageConnectionString"];
            var talkLog = await TableStorage<TalkLog>.CreateAsync(connectionString, "ChildsNotification");

            var app = new LineBotApp(line, talkLog);
            await app.RunAsync(events);
            return new OkResult();
        }
    }
}
