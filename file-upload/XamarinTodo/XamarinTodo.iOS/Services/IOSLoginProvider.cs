using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using XamarinTodo.iOS.Services;
using XamarinTodo.Services;
using UIKit;
using System.Diagnostics;

[assembly: Xamarin.Forms.Dependency(typeof(IOSLoginProvider))]
namespace XamarinTodo.iOS.Services
{
    class IOSLoginProvider : ILoginProvider
    {
        public async Task LoginAsync(MobileServiceClient client)
        {
            Debug.WriteLine($"[IOSLoginProvider] LoginAsync");
            await client.LoginAsync(UIApplication.SharedApplication.KeyWindow.RootViewController, MobileServiceAuthenticationProvider.Google);
            Debug.WriteLine($"[DroidLoginProvider] UserId = {client.CurrentUser.UserId} Token = {client.CurrentUser.MobileServiceAuthenticationToken}");
        }

        public async Task LogoutAsync(MobileServiceClient client)
        {
            System.Diagnostics.Debug.WriteLine($"[IOSLoginProvider] LogoutAsync");
            await client.LogoutAsync();
        }
    }
}
