using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;

namespace Client.UWP.Services
{
    public class EntityData
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "createdAt")]
        public DateTimeOffset CreatedAt { get; set; }

        [UpdatedAt]
        public DateTimeOffset UpdatedAt { get; set; }

        [Version]
        public string Version { get; set; }

        /// <summary>
        /// Determines if two elements are equivalent
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Equals(EntityData item)
        {
            return item.Id == Id;
        }
    }
}
