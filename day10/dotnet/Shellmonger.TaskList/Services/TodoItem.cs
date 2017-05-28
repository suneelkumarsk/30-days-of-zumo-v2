using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace Shellmonger.TaskList.Services
{
    public class TodoItem
    {
        public string Id { get; set; }

        [Version]
        public string Version { get; set; }

        [JsonProperty("text")]
        public string Title { get; set; }

        [JsonProperty("complete")]
        public bool Completed { get; set; }

        [JsonProperty("shared")]
        public bool Shared { get; set; }

        [JsonIgnore]
        public bool NotShared => !Shared;
    }
}
