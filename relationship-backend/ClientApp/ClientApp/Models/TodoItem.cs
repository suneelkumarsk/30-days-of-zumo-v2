using ClientApp.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

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

        [ForeignKey("TagId")]
        public Tag Tag { get; set; }
    }
}
