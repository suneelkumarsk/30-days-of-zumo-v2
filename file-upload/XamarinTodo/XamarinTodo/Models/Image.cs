using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;

namespace XamarinTodo.Models
{
    public class Image
    {
        public string Filename { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

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

