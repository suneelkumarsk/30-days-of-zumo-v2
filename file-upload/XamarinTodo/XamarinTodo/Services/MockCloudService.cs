using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using XamarinTodo.Models;

namespace XamarinTodo.Services
{
    public class MockCloudService : ICloudService
    {
        List<TodoItem> items { get; } = new List<TodoItem>();

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        /// <summary>
        /// Delete the listed item
        /// </summary>
        /// <param name="item">the item to delete</param>
        /// <returns>true if the item existed before and was deleted</returns>
        public async Task<bool> DeleteItemAsync(TodoItem item)
        {
            Debug.WriteLine($"[MockCloudService] DeleteItemAsync {item.Id}");
            var itemToDelete = items.FirstOrDefault(x => x.Id.Equals(item.Id));
            if (itemToDelete != null)
            {
                items.Remove(itemToDelete);
                Debug.WriteLine("[MockCloudService] DeleteItemAsync success");
                return true;
            }
            Debug.WriteLine("[MockCloudService] DeleteItemAsync failure");
            return false;
        }

        public async Task<IEnumerable<TodoItem>> GetAllItemsAsync()
        {
            Debug.WriteLine($"[MockCloudService] GetAllItemsAsync {items.Count} items returned");
            return items.AsEnumerable();
        }

        public async Task InitializeAsync()
        {
            Debug.WriteLine($"[MockCloudService] InitializeAsync called");
            return;
        }

        public async Task LoginAsync()
        {
            Debug.WriteLine($"[MockCloudService] LoginAsync called");
            return;
        }

        public async Task LogoutAsync()
        {
            Debug.WriteLine($"[MockCloudService] LogoutAsync called");
            return;
        }

        public async Task SynchronizeServiceAsync()
        {
            Debug.WriteLine($"[MockCloudService] SynchronizeServiceAsync called");
            return;
        }

        /// <summary>
        /// Insert or Update the new item
        /// </summary>
        /// <param name="item">the item to upsert</param>
        /// <returns>the completed item</returns>
        public async Task<TodoItem> UpsertItemAsync(TodoItem item)
        {
            Debug.WriteLine($"[MockCloudService] UpsertItemAsync for {item.Text} called");
            if (item.Id == null)
            {
                Debug.WriteLine($"[MockCloudService] Upsert is an Insert");
                item.Id = Guid.NewGuid().ToString("N");
                item.CreatedAt = DateTimeOffset.Now;
                item.UpdatedAt = DateTimeOffset.Now;
                item.AzureVersion = item.UpdatedAt.GetHashCode().ToString();
                items.Add(item);
            }
            else
            {
                Debug.WriteLine($"[MockCloudService] Upsert is an Update");
                item.UpdatedAt = DateTimeOffset.Now;
                item.AzureVersion = item.UpdatedAt.GetHashCode().ToString();
                items.Remove(items.FirstOrDefault(x => x.Id.Equals(item.Id)));
                items.Add(item);
            }
            Debug.WriteLine($"[MockCloudService] Id={item.Id} UpdatedAt={item.UpdatedAt} Text={item.Text} Complte={item.Complete} Version={item.AzureVersion}");
            return item;
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    }
}
