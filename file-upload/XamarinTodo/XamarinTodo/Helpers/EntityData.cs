using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;

namespace XamarinTodo.Helpers
{
    /// <summary>
    /// This is a direct copy of the EntityData class from Azure Mobile Apps, with
    /// the exception that the Deleted property is not included.  The Deleted property
    /// is handled by the offline sync database and does not need to be exposed.
    /// </summary>
    public class EntityData
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "createdAt")]
        public DateTimeOffset? CreatedAt { get; set; }

        [UpdatedAt]
        public DateTimeOffset? UpdatedAt { get; set; }

        [Version]
        public string AzureVersion { get; set; }
    }
}
