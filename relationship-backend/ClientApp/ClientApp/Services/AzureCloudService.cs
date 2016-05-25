using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientApp.Models;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;

namespace ClientApp.Services
{
    public class AzureCloudService : ICloudService
    {
        private IMobileServiceSyncTable<TodoItem> itemTable;
        private IMobileServiceSyncTable<Tag> tagTable;
        private bool isInitialized;
        private MobileServiceClient client;

        public AzureCloudService()
        {
            client = new MobileServiceClient(Helpers.Locations.AppServiceUrl)
            {
                SerializerSettings = new MobileServiceJsonSerializerSettings { CamelCasePropertyNames = true }
            };
        }

        public async Task InitializeAsync()
        {
            if (isInitialized)
                return;

            var store = new MobileServiceSQLiteStore("client.db");
            store.DefineTable<TodoItem>();
            store.DefineTable<Tag>();

            await client.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());
            itemTable = client.GetSyncTable<TodoItem>();
            tagTable = client.GetSyncTable<Tag>();

            isInitialized = true;
        }

        public async Task SynchronizeServiceAsync()
        {
            await InitializeAsync();
            await client.SyncContext.PushAsync();
            await tagTable.PullAsync("alltags", tagTable.CreateQuery());
            await itemTable.PullAsync("allitems", itemTable.CreateQuery());
        }

        public async Task<IEnumerable<TodoItem>> GetAllItemsAsync()
        {
            await InitializeAsync();
            await SynchronizeServiceAsync();
            return await itemTable.ToEnumerableAsync();
        }

        public async Task<TodoItem> UpsertItemAsync(TodoItem item)
        {
            await InitializeAsync();
            if (item.Id == null)
            {
                item.Id = Guid.NewGuid().ToString();
                await itemTable.InsertAsync(item);
            }
            else
            {
                await itemTable.UpdateAsync(item);
            }
            await SynchronizeServiceAsync();
            return await itemTable.LookupAsync(item.Id);
        }

        public async Task<bool> DeleteItemAsync(TodoItem item)
        {
            await InitializeAsync();
            try
            {
                await itemTable.DeleteAsync(item);
                await SynchronizeServiceAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<Tag>> GetAllTagsAsync()
        {
            await InitializeAsync();
            await SynchronizeServiceAsync();
            return await tagTable.ToEnumerableAsync();
        }

        public async Task<Tag> UpsertTagAsync(Tag tag)
        {
            await InitializeAsync();
            if (tag.Id == null)
            {
                tag.Id = Guid.NewGuid().ToString();
                await tagTable.InsertAsync(tag);
            }
            else
            {
                await tagTable.UpdateAsync(tag);
            }
            await SynchronizeServiceAsync();
            return await tagTable.LookupAsync(tag.Id);
        }

        public async Task<bool> DeleteTagAsync(Tag tag)
        {
            await InitializeAsync();
            try
            {
                await tagTable.DeleteAsync(tag);
                await SynchronizeServiceAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
