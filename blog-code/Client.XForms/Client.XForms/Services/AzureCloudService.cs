using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Client.XForms.Models;

namespace Client.XForms.Services
{
    public class AzureCloudService : ICloudService
    {
        public Task<TodoItem> AddOrUpdateItemAsync(TodoItem item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteItemAsync(TodoItem item)
        {
            throw new NotImplementedException();
        }

        public Task<TodoItem> GetItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TodoItem>> GetItemsAsync()
        {
            throw new NotImplementedException();
        }

        public Task InitializeAsync()
        {
            throw new NotImplementedException();
        }

        public Task LoginAsync()
        {
            throw new NotImplementedException();
        }

        public Task LogoutAsync()
        {
            throw new NotImplementedException();
        }

        public Task SyncAsync()
        {
            throw new NotImplementedException();
        }
    }
}
