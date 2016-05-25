using Microsoft.Azure.Mobile.Server;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mobile.Backend.DataObjects
{
    public class Tag : EntityData
    {
        public string TagName { get; set; }
    }

    public class TodoItem : EntityData
    {
        public string Text { get; set; }

        public bool Complete { get; set; }

        #region Relationships
        public string TagId { get; set; }

        [ForeignKey("TagId")]
        public Tag Tag { get; set; }
        #endregion
    }
}