using ClientApp.Helpers;

namespace ClientApp.Models
{
    public class Tag : EntityData
    {
        public string TagName { get; set; }
    }

    public class TodoItem : EntityData
    {
        public string Text { get; set; }
        public bool Complete { get; set; }

        public string TagId { get; set; }
    }
}
