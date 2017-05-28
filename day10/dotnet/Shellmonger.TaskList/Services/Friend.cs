using Newtonsoft.Json;

namespace Shellmonger.TaskList.Services
{
    public class Friend
    {
        public string Id { get; set; }

        [JsonProperty("viewer")]
        public string Viewer { get; set; }
    }
}
