using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;

namespace XamarinTodo.Services
{
    public interface ILoginProvider
    {
        Task LoginAsync(MobileServiceClient client);

        Task LogoutAsync(MobileServiceClient client);
    }
}
