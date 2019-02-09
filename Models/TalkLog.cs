using Microsoft.WindowsAzure.Storage.Table;

namespace childs_notification.Models
{
    public class TalkLog : TableEntity
    {
        [IgnoreProperty]
        public string MessageType { get { return PartitionKey; } set { PartitionKey = value; } }

        [IgnoreProperty]
        public string ReplyToken { get { return RowKey; } set { RowKey = value; } }

        public string UserId { get; set; }

        public string Message { get; set; }
    }
}
