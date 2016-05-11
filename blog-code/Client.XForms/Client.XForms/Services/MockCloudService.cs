using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.XForms.Models;

namespace Client.XForms.Services
{
    /// <summary>
    /// A mock version of the Azure Cloud Service
    /// </summary>
    class MockCloudService : ICloudService
    {
        // Holder for the items in the store
        private List<TodoItem> items { get; set; } = new List<TodoItem>();
        // Holder for the logged in flag
        private bool loggedIn = false;

        public Task<TodoItem> AddOrUpdateItemAsync(TodoItem item)
        {
            if (item.Id == null)
            {
                item.Id = Guid.NewGuid().ToString("N");
                item.CreatedAt = DateTimeOffset.Now;
                item.UpdatedAt = DateTimeOffset.Now;
                item.AzureVersion = "1";
                items.Add(item);
            }
            else
            {
                item.UpdatedAt = DateTimeOffset.Now;
                item.AzureVersion = (Int64.Parse(item.AzureVersion) + 1).ToString();
                var idx = items.FirstOrDefault(i => i.Id.Equals(item.Id));
                items.Remove(idx);
                items.Add(item);
            }
            return Task.FromResult(item);
        }

        public Task<bool> DeleteItemAsync(TodoItem item)
        {
            items.Remove(item);
            return Task.FromResult(true);
        }

        public Task<TodoItem> GetItemAsync(string id)
        {
            var item = items.FirstOrDefault(i => i.Id.Equals(id));
            return Task.FromResult(item);
        }

        public Task<IEnumerable<TodoItem>> GetItemsAsync()
        {
            IEnumerable<TodoItem> result = items.AsEnumerable();
            return Task.FromResult(result);
        }

        public Task InitializeAsync()
        {
            // Short-circuit - we only want some items when we get
            if (items.Count > 0)
                return null;

            // List of "first-run" items we need to add
            items.Add(new TodoItem
            {
                Title = "Get a haircut",
                Complete = false
            });

            // Mock service does everything sync.
            return null;
        }

        public Task LoginAsync()
        {
            if (loggedIn)
                return null;

            loggedIn = true;
            return null;
        }

        public Task LogoutAsync()
        {
            if (!loggedIn)
                return null;

            loggedIn = false;
            return null;
        }

        public Task SyncAsync()
        {
            return null;
        }
    }
}
