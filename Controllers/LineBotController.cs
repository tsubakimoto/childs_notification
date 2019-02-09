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
            var blobStorage = await BlobStorage.CreateAsync(connectionString, "linebotcontainer");
            var eventSourceState = await TableStorage<EventSourceState>.CreateAsync(connectionString, "eventsourcestate");

            var app = new LineBotApp(line, eventSourceState, blobStorage);
            await app.RunAsync(events);
            return new OkResult();
        }
    }
}
