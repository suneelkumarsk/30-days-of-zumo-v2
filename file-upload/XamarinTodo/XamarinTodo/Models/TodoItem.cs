using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;

namespace XamarinTodo.Models
{
    /// <summary>
    /// The TodoItem - this must match the TodoItem in the Backend/DataObjects directory
    /// precisely.  Note that the EntityData in the DTO and the EntityData here are actually
    /// different to account for the difference in the Deleted property.
    /// </summary>
    public class TodoItem
    {
        public string Text { get; set; }
        public bool Complete { get; set; }

        // From EntityData Base Type
        // See https://github.com/Azure/azure-mobile-apps-net-files-client/issues/38
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
