using Client.UWP.Services;
using Newtonsoft.Json;

namespace Client.UWP.Models
{
    class TodoItem : EntityData
    {
        public string Title { get; set; }

        public bool Complete { get; set; }
    }
}
