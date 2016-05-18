using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using XamarinTodo.Models;

namespace XamarinTodo.Services
{
    public class AzureCloudService : ICloudService
    {

        private IMobileServiceSyncTable<TodoItem> itemTable;
        private ILoginProvider loginProvider;
        private bool isInitialized;

        public AzureCloudService()
        {
            MobileService = new MobileServiceClient(Helpers.Locations.AppServiceUrl)
            {
                SerializerSettings = new MobileServiceJsonSerializerSettings { CamelCasePropertyNames = true }
            };

            // Get the login provider for this service
            loginProvider = DependencyService.Get<ILoginProvider>();
        }

        public MobileServiceClient MobileService { get; set; }

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

        public async Task<IEnumerable<TodoItem>> GetAllItemsAsync()
        {
            await InitializeAsync();
            await SynchronizeServiceAsync();
            return await itemTable.ToEnumerableAsync();
        }

        public async Task InitializeAsync()
        {
            if (isInitialized)
                return;

            var store = new MobileServiceSQLiteStore("xamarintodo.db");
            store.DefineTable<TodoItem>();

            await MobileService.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());
            itemTable = MobileService.GetSyncTable<TodoItem>();

            isInitialized = true;
        }

        public async Task LoginAsync()
        {
            await loginProvider.LoginAsync(MobileService);
        }

        public async Task LogoutAsync()
        {
            await loginProvider.LogoutAsync(MobileService);
        }

        public async Task SynchronizeServiceAsync()
        {
            await InitializeAsync();
            await MobileService.SyncContext.PushAsync();
            await itemTable.PullAsync("allitems", itemTable.CreateQuery());
        }

        public async Task<TodoItem> UpsertItemAsync(TodoItem item)
        {
            await InitializeAsync();

            if (item.Id == null)
            {
                item.Id = Guid.NewGuid().ToString("N");
                await itemTable.InsertAsync(item);
            }
            else
            {
                await itemTable.UpdateAsync(item);
            }

            await SynchronizeServiceAsync();
            return await itemTable.LookupAsync(item.Id);
        }
    }
}