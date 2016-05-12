using Client.XForms.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.XForms.Services
{
    public interface ICloudService
    {
        Task LoginAsync();

        Task LogoutAsync();

        Task<IEnumerable<Todoitem>> GetAllItemsAsync();

        Task<Todoitem> AddOrUpdateItemAsync(Todoitem item);

        Task DeleteItemAsync(Todoitem item);

        Task SynchronizeItemsAsync();
    }
}
