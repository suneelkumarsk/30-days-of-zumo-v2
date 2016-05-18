using System.Collections.Generic;
using System.Threading.Tasks;
using XamarinTodo.Models;

namespace XamarinTodo.Services
{
    public interface ICloudService
    {
        Task InitializeAsync();

        Task<IEnumerable<TodoItem>> GetAllItemsAsync();

        Task<TodoItem> UpsertItemAsync(TodoItem item);

        Task<bool> DeleteItemAsync(TodoItem item);

        Task SynchronizeServiceAsync();

        Task LoginAsync();

        Task LogoutAsync();

        Task<StorageTokenViewModel> GetStorageToken();
    }
}
