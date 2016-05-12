using Client.XForms.Services;
using Newtonsoft.Json;

namespace Client.XForms.Models
{
    public class TodoItem : EntityData
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "complete")]
        public bool Complete { get; set; }

    }
}
