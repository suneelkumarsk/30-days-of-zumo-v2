using Client.XForms.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.XForms.Services
{
    public interface ICloudService
    {
        /// <summary>
        /// Initialize the cloud service
        /// </summary>
        Task InitializeAsync();

        /// <summary>
        /// Log in to the cloud service
        /// </summary>
        Task LoginAsync();

        /// <summary>
        /// Log out of the cloud service
        /// </summary>
        Task LogoutAsync();

        /// <summary>
        /// Get the list of current items from the cloud service
        /// </summary>
        /// <returns>An IEnumerable of all the items</returns>
        Task<IEnumerable<TodoItem>> GetItemsAsync();

        /// <summary>
        /// Get a single item by Id
        /// </summary>
        /// <param name="id">The Id of the item</param>
        /// <returns>The item</returns>
        Task<TodoItem> GetItemAsync(string id);

        /// <summary>
        /// Add or Update a single item
        /// </summary>
        /// <param name="item">The item to add or update</param>
        Task<TodoItem> AddOrUpdateItemAsync(TodoItem item);

        /// <summary>
        /// Delete the specified item
        /// </summary>
        /// <param name="item">The item to delete</param>
        /// <returns>true if deletion is successful</returns>
        Task<bool> DeleteItemAsync(TodoItem item);

        /// <summary>
        /// Sync changes to the cloud service
        /// </summary>
        Task SyncAsync();
    }
}
