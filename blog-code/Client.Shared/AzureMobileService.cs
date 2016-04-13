using Microsoft.WindowsAzure.MobileServices;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Client.Shared
{
    public class MobileAppService
    {
        /// <summary>
        /// Singleton Pattern: The Azure Mobile Service Instance
        /// </summary>
        static MobileAppService instance = new MobileAppService();

        /// <summary>
        /// The name of the service - without the .azurewebsites.net
        /// </summary>
        static string serviceName = "shell-4ca00908-6291-4333-8894-0ffd5291b110";

        #region Private Variables
        /// <summary>
        /// The actual backend connection to the service
        /// </summary>
        MobileServiceClient client;

        /// <summary>
        /// Table reference
        /// </summary>
        IMobileServiceTable<TodoItem> tasksTable;
        #endregion

        /// <summary>
        /// Create a new connection and initialize for offline-sync (if required)
        /// </summary>
        private MobileAppService()
        {
            this.client = new MobileServiceClient($"https://{serviceName}.azurewebsites.net");
            this.tasksTable = client.GetTable<TodoItem>();
        }

        #region Singleton Pattern
        /// <summary>
        /// Returns the defined instance of the service
        /// </summary>
        public static MobileAppService Instance
        {
            get { return instance; }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Read-only reference to the Mobile Service Client
        /// </summary>
        public MobileServiceClient Client
        {
            get { return client; }
        }
        #endregion

        #region Tasks Table Handler
        public async Task<ObservableCollection<TodoItem>> GetTasksAsync(bool syncItems = false)
        {
            try
            {
                IEnumerable<TodoItem> items = await tasksTable
                    .Where(item => !item.Complete)
                    .OrderBy(item => item.UpdatedAt)
                    .ToEnumerableAsync();

                return new ObservableCollection<TodoItem>(items);
            }
            catch (MobileServiceInvalidOperationException serviceException)
            {
                Debug.WriteLine($"GetTasksAsync Error: {serviceException.Message}");
                throw new AzureSyncException("GetTasksAsync", serviceException);
            }
        }

        public async Task SaveTaskAsync(TodoItem item)
        {
            try
            {
                if (item.Id == null)
                {
                    await tasksTable.InsertAsync(item);
                }
                else
                {
                    await tasksTable.UpdateAsync(item);
                }
            }
            catch (MobileServicePreconditionFailedException<TodoItem> preconditionException)
            {
                Debug.WriteLine($"SaveTaskAsync Error: 412 on item {item}");
                throw new ResolveConflictException<TodoItem>(item, preconditionException.Item);
            }
            catch (MobileServiceConflictException<TodoItem> conflictException)
            {
                Debug.WriteLine($"SaveTaskAsync Error: 409 on item {item}");
                throw new ResolveConflictException<TodoItem>(item, conflictException.Item);
            }
        }
        #endregion
    }
}
