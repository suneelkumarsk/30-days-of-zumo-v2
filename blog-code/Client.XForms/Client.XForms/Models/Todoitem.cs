using Client.XForms.Helpers;
using Newtonsoft.Json;

namespace Client.XForms.Models
{
    public class Todoitem : EntityData
    {
        [JsonProperty(PropertyName = "title")]
        public string Text { get; set; }

        public bool Complete { get; set; }
    }
}
