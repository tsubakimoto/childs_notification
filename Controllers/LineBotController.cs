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

namespace childs_notification.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class LineBotController : Controller
    {
        private static LineMessagingClient lineMessagingClient;
        private readonly ILogger logger;
        AppSettings appsettings;
        public LineBotController(IOptions<AppSettings> options, ILogger<LineBotController> _logger)
        {
            appsettings = options.Value;
            var channelAccessToken = Environment.GetEnvironmentVariable("ChannelAccessToken") ?? appsettings.LineSettings.ChannelAccessToken;
            lineMessagingClient = new LineMessagingClient(channelAccessToken);
            logger = _logger;
        }

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]JToken req)
        {
            logger.LogInformation(req.ToString());
            var events = WebhookEventParser.Parse(req.ToString());
            var connectionString = Environment.GetEnvironmentVariable("StorageConnectionString") ?? appsettings.LineSettings.StorageConnectionString;
            var blobStorage = await BlobStorage.CreateAsync(connectionString, "linebotcontainer");
            var eventSourceState = await TableStorage<EventSourceState>.CreateAsync(connectionString, "eventsourcestate");

            var app = new LineBotApp(lineMessagingClient, eventSourceState, blobStorage);
            await app.RunAsync(events);
            return new OkResult();
        }
    }
}
