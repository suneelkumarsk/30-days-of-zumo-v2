using Microsoft.Azure.Mobile.Server;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.dotnet.DataObjects
{
    [Table("TodoItem", Schema="mobile")]
    public class TodoItem : EntityData
    {
        public string UserId { get; set; }

        public string Title { get; set; }

        public bool Complete { get; set; }

        // When asked for the string representation, return the JSON
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}