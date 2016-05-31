using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Files;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Xamarin.Forms;
using XamarinTodo.Models;

namespace XamarinTodo.Services
{
	public class AzureCloudService : ICloudService
    {

        private IMobileServiceSyncTable<TodoItem> itemTable;
		private IFileProvider fileProvider;
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
			fileProvider = DependencyService.Get<IFileProvider>();
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

            // Initialize File Sync
            MobileService.InitializeFileSyncContext(new TodoItemFileSyncHandler(), store);

            // Initialize the sync context
            await MobileService.SyncContext.InitializeAsync(store,
                new MobileServiceSyncHandler(),
                StoreTrackingOptions.NotifyLocalAndServerOperations);

            // Get a reference to the sync table
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
            await itemTable.PushFileChangesAsync();
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

        public Task<StorageTokenViewModel> GetStorageToken()
            => MobileService.InvokeApiAsync<StorageTokenViewModel>("GetStorageToken", HttpMethod.Get, null);

        public async Task DownloadItemFileAsync(MobileServiceFile file)
        {
            var path = await fileProvider.GetLocalFilePathAsync(file.ParentId, file.Name);
            await fileProvider.DownloadFileAsync(itemTable, file, path);
        }

        public async Task<MobileServiceFile> AddItemImageAsync(TodoItem item, string image)
        {
            var path = await fileProvider.CopyItemFileAsync(item.Id, image);
            var fileName = Path.GetFileName(path);
            return await itemTable.AddFileAsync(item, fileName);
        }

        public async Task<IEnumerable<MobileServiceFile>> GetItemImageFilesAsync(TodoItem item)
            => await itemTable.GetFilesAsync(item);

    }
}