using Microsoft.WindowsAzure.Storage.Table;

namespace childs_notification.Models
{
    public class EventSourceLocation : EventSourceState
    {
        public string Location { get; set; }

        public EventSourceLocation() { }
    }
}