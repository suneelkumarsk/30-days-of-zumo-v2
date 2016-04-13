using Microsoft.WindowsAzure.MobileServices;
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
        public static AzureCloudProvider Instance
        {
            get { return instance; }
        }
        #endregion

        private IMobileServiceClient client;

        /// <summary>
        /// Constructor - creates a connection to the backend.
        /// </summary>
        private AzureCloudProvider()
        {
            var appID = "shell-4ca00908-6291-4333-8894-0ffd5291b110";
            var clientUri = $"https://{appID}.azurewebsites.net";

            Debug.WriteLine($"[AzureCloudProvider#constructor] Initializing connection to {clientUri}");
            this.client = new MobileServiceClient(clientUri);

            Debug.WriteLine($"[AzureCloudProvider#constructor] Initialiation Complete");
        }

        /// <summary>
        /// Property: Returns the current client
        /// </summary>
        public IMobileServiceClient Client
        {
            get { return client; }
        }

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
        /// Get a reference to a table
        /// </summary>
        /// <typeparam name="T">The model in the table</typeparam>
        /// <returns>An AzureDataTable reference</returns>
        public AzureDataTable<T> GetTable<T>() where T:EntityData
        {
            return new AzureDataTable<T>(client.GetTable<T>());
        }

    }
}
