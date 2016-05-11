using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;

namespace Client.XForms.Services
{
    public class EntityData
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "createdAt")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty(PropertyName = "updatedAt")]
        public DateTimeOffset UpdatedAt { get; set; }

        [Version]
        public string AzureVersion { get; set; }

    }
}
