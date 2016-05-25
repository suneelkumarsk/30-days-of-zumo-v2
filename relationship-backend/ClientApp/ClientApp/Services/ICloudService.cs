using ClientApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientApp.Services
{
    public interface ICloudService
    {
        Task InitializeAsync();
        Task SynchronizeServiceAsync();

        #region TodoItem CRUD
        Task<IEnumerable<TodoItem>> GetAllItemsAsync();
        Task<TodoItem> UpsertItemAsync(TodoItem item);
        Task<bool> DeleteItemAsync(TodoItem item);
        #endregion

        #region Tag CRUD
        Task<IEnumerable<Tag>> GetAllTagsAsync();

        Task<Tag> GetTagByIdAsync(string id);

        Task<Tag> UpsertTagAsync(Tag item);
        Task<bool> DeleteTagAsync(Tag item);
        #endregion
    }
}
