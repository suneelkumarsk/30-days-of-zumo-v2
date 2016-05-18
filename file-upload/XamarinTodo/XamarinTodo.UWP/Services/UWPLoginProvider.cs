using Microsoft.WindowsAzure.MobileServices;
using System.Diagnostics;
using System.Threading.Tasks;
using XamarinTodo.Services;
using XamarinTodo.UWP.Services;

[assembly: Xamarin.Forms.Dependency(typeof(UWPLoginProvider))]

namespace XamarinTodo.UWP.Services
{
    internal class UWPLoginProvider : ILoginProvider
    {
        public async Task LoginAsync(MobileServiceClient client)
        {
            Debug.WriteLine($"[UWPLoginProvider] LoginAsync");
            await client.LoginAsync(MobileServiceAuthenticationProvider.Google);
            Debug.WriteLine($"[UWPLoginProvider] UserId = {client.CurrentUser.UserId} Token = {client.CurrentUser.MobileServiceAuthenticationToken}");
        }

        public async Task LogoutAsync(MobileServiceClient client)
        {
            Debug.WriteLine($"[UWPLoginProvider] LogoutAsync");
            await client.LogoutAsync();
        }
    }
}