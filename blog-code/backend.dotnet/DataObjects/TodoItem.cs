using Microsoft.Azure.Mobile.Server;

namespace backend.dotnet.DataObjects
{
    public class TodoItem : EntityData
    {
        public string UserId { get; set; }

        public string Text { get; set; }

        public bool Complete { get; set; }
    }
}