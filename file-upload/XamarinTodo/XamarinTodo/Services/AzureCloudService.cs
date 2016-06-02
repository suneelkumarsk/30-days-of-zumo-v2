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
        private IMobileServiceSyncTable<Models.Image> imageTable;
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
            store.DefineTable<Models.Image>();

            // Initialize File Sync
            MobileService.InitializeFileSyncContext(new TodoItemFileSyncHandler(), store);

            // Initialize the sync context
            await MobileService.SyncContext.InitializeAsync(store,
                new MobileServiceSyncHandler(),
                StoreTrackingOptions.NotifyLocalAndServerOperations);

            // Get a reference to the sync table
            itemTable = MobileService.GetSyncTable<TodoItem>();
            imageTable = MobileService.GetSyncTable<Models.Image>();

            isInitialized = true;

            // Add a single item into the image table if there aren't any
            // This forces creation.
            var list = await imageTable.ToListAsync();
            if (list.Count == 0)
            {
                var image = new Models.Image
                {
                    Id = Guid.NewGuid().ToString(),
                    Filename = "dummy",
                    Height = 0,
                    Width = 0
                };
                await imageTable.InsertAsync(image);
                await SynchronizeServiceAsync();
            }
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
            await imageTable.PullAsync("allimages", imageTable.CreateQuery());
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
            var msFile = await itemTable.AddFileAsync(item, fileName);
            return msFile;
        }

        public async Task<IEnumerable<MobileServiceFile>> GetItemImageFilesAsync(TodoItem item)
            => await itemTable.GetFilesAsync(item);

    }
}