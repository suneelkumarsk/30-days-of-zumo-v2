using Client.UWP.Services;
using Newtonsoft.Json;

namespace Client.UWP.Models
{
    class TodoItem : EntityData
    {
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "complete")]
        public bool Completed { get; set; }
    }
}
