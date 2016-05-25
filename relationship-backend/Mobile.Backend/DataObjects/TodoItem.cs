using Microsoft.Azure.Mobile.Server;
using System.Collections.Generic;
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

        public virtual ICollection<Tag> Tags { get; set; }
    }
}