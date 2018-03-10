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
            // lineMessagingClient = new LineMessagingClient(appsettings.LineSettings.ChannelAccessToken);
            lineMessagingClient = new LineMessagingClient(Environment.GetEnvironmentVariable("ChannelAccessToken"));
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
            var to = Environment.GetEnvironmentVariable("RoomId");
            var messages = new List<ISendMessage>
            {
                new TextMessage("Bot said: Hello!!")
            };
            await lineMessagingClient.PushMessageAsync(to, messages);
            return new OkResult();
        }
    }
}
