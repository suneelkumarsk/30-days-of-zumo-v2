using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;

namespace Client.Shared
{
    public class TodoItem
    {
        #region Azure Mobile Properties
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "createdAt")]
        public DateTimeOffset CreatedAt { get; set; }

        [UpdatedAt]
        public DateTimeOffset UpdatedAt { get; set; }

        [Version]
        public string Version { get; set; }
        #endregion

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "complete")]
        public bool Complete { get; set; }

        #region ToString
        public override string ToString()
        {
            return $"{{\"text\":\"{Text}\",\"complete\":{Complete},\"updatedAt\":\"{UpdatedAt}\"}}";
        }
        #endregion

        #region Resolver
        /// <summary>
        /// Used by the conflict resolver
        /// </summary>
        /// <returns>a string with all fields in it</returns>
        public string Resolver()
        {
            return $"{{\"text\":\"{Text}\",\"complete\":{Complete}}}";
        }
        #endregion
    }
}