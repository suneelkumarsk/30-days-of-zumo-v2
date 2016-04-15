using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Client.UWP.Services
{
    public class AzureDataTable<T> where T: EntityData
    {
        private AzureCloudProvider provider;
        private bool isSyncTable = false;

        private IMobileServiceSyncTable<T> syncTable;
        private IMobileServiceTable<T> dataTable;

        private ObservableCollection<T> dataView;


        public AzureDataTable(AzureCloudProvider provider)
        {
            this.provider = provider;
            this.isSyncTable = provider.IsSyncTable(typeof(T).Name);

            if (isSyncTable)
            {
                this.syncTable = provider.Client.GetSyncTable<T>();
            }
            else
            {
                this.dataTable = provider.Client.GetTable<T>();
            }
        }

        public async Task SaveAsync(T item)
        {
            try
            {
                if (item.Id == null)
                {
                    if (isSyncTable)
                        await syncTable.InsertAsync(item);
                    else
                        await dataTable.InsertAsync(item);
                    dataView.Add(item);
                }
                else
                {
                    if (isSyncTable)
                        await syncTable.UpdateAsync(item);
                    else
                        await dataTable.UpdateAsync(item);
                    dataView.Remove(item);
                    dataView.Add(item);
                }
            }
            catch (MobileServiceInvalidOperationException exception)
            {
                throw new CloudTableOperationFailed(exception.Message, exception);
            }
        }

        public async Task DeleteAsync(T item)
        {
            try
            {
                if (isSyncTable)
                    await syncTable.DeleteAsync(item);
                else
                    await dataTable.DeleteAsync(item);
                dataView.Remove(item);
            }
            catch (MobileServiceInvalidOperationException exception)
            {
                throw new CloudTableOperationFailed(exception.Message, exception);
            }
        }

        public async Task<ObservableCollection<T>> RefreshAsync(bool syncItems = true)
        {
            Debug.WriteLine($"[AzureDataTable$RefreshAsync] Entry");
            try
            {
                if (syncItems && isSyncTable)
                {
                    Debug.WriteLine($"[AzureDataTable$RefreshAsync] Updating Offline Sync Cache");
                    await this.SyncOfflineCacheAsync();
                }
                Debug.WriteLine($"[AzureDataTable$RefreshAsync] Requesting Items");
                if (isSyncTable)
                {
                    List<T> items = await syncTable.OrderBy(item => item.UpdatedAt).ToListAsync();
                    dataView = new ObservableCollection<T>(items);
                }
                else
                {
                    List<T> items = await dataTable.OrderBy(item => item.UpdatedAt).ToListAsync();
                    dataView = new ObservableCollection<T>(items);
                }
                Debug.WriteLine($"[AzureDataTable$RefreshAsync] {dataView.Count} items available");
                return dataView;
            }
            catch (MobileServiceInvalidOperationException exception)
            {
                throw new CloudTableOperationFailed(exception.Message, exception);
            }
        }

        public async Task SyncOfflineCacheAsync()
        {
            string queryName = $"incremental_sync_{typeof(T).Name}";
            try
            {
                await provider.Client.SyncContext.PushAsync();
                await syncTable.PullAsync(queryName, syncTable.CreateQuery());
            }
            catch (MobileServicePushFailedException exception)
            {
                if (exception.PushResult != null)
                {
                    foreach (var error in exception.PushResult.Errors)
                    {
                        await ResolveConflictAsync(error);
                    }
                }
            }
        }

        private async Task ResolveConflictAsync(MobileServiceTableOperationError error)
        {
            Debug.WriteLine($"Resolve Conflict for Item: {error.Item}");

            var serverItem = error.Result.ToObject<T>();
            var localItem = error.Item.ToObject<T>();

            if (serverItem.Equals(localItem))
            {
                // Items are the same, so ignore the conflict
                await error.CancelAndDiscardItemAsync();
            }
            else
            {
                // Always take the client
                localItem.Version = serverItem.Version;
                await error.UpdateOperationAsync(JObject.FromObject(localItem));
            }
        }
    }
}