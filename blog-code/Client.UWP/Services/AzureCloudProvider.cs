using Client.UWP.Models;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Client.UWP.Services
{
    public class AzureCloudProvider
    {
        #region Singleton Pattern
        /// <summary>
        /// Singleton Pattern instance storage
        /// </summary>
        private static AzureCloudProvider instance = new AzureCloudProvider();

        /// <summary>
        /// Property for getting the current cloud provider
        /// </summary>
        public static AzureCloudProvider Instance => instance;
        #endregion

        private IMobileServiceClient client;
        private List<string> syncTables = new List<string>();

        /// <summary>
        /// Constructor - creates a connection to the backend.
        /// </summary>
        private AzureCloudProvider()
        {
            var appID = "shellmonger-demo";
            var clientUri = $"https://{appID}.azurewebsites.net";

            Debug.WriteLine($"[AzureCloudProvider#constructor] Initializing connection to {clientUri}");
            this.client = new MobileServiceClient(clientUri);

            Debug.WriteLine($"[AzureCloudProvider#constructor] Initialization Complete");
        }

        /// <summary>
        /// Property: Returns the current client
        /// </summary>
        public IMobileServiceClient Client => client;

        /// <summary>
        /// Login to the cloud resource
        /// </summary>
        /// <returns>async task</returns>
        public async Task LoginAsync()
        {
            try
            {
                await client.LoginAsync("aad");
            }
            catch (MobileServiceInvalidOperationException exception)
            {
                throw new CloudAuthenticationFailedException(exception.Message, exception);
            }
        }

        /// <summary>
        /// Log out of the cloud resource
        /// </summary>
        /// <returns>async task</returns>
        public async Task LogoutAsync()
        {
            try
            {
                await client.LogoutAsync();
            }
            catch (MobileServiceInvalidOperationException exception)
            {
                throw new CloudAuthenticationFailedException(exception.Message, exception);
            }
        }

        /// <summary>
        /// Initialize the Offline Sync Store
        /// </summary>
        /// <returns>async task</returns>
        public async Task InitializeOfflineSync()
        {
            var store = new MobileServiceSQLiteStore("localstore.db");

            store.DefineTable<TodoItem>();
            syncTables.Add(typeof(TodoItem).Name);

            await client.SyncContext.InitializeAsync(store);
        }

        /// <summary>
        /// Get a reference to a table
        /// </summary>
        /// <typeparam name="T">The model in the table</typeparam>
        /// <returns>An AzureDataTable reference</returns>
        public AzureDataTable<T> GetTable<T>() where T : EntityData => new AzureDataTable<T>(this);

        /// <summary>
        /// Determines if the table name is a sync tables
        /// </summary>
        /// <param name="className">The name of the table</param>
        /// <returns>boolean</returns>
        public bool IsSyncTable(string className) => syncTables.Contains(className);

    }
}
