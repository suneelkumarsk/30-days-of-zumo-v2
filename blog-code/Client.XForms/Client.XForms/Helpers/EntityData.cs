using System;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.MobileServices;

namespace Client.XForms.Helpers
{
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
