using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using XamarinTodo.Services;
using XamarinTodo.Droid.Services;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency (typeof(DroidLoginProvider))]
namespace XamarinTodo.Droid.Services
{
    class DroidLoginProvider : ILoginProvider
    {
        Context _context;

        public void Initialize(Context context)
        {
            this._context = context;
        }

        public async Task LoginAsync(MobileServiceClient client)
        {
            System.Diagnostics.Debug.WriteLine($"[DroidLoginProvider] LoginAsync");
            await client.LoginAsync(this._context, MobileServiceAuthenticationProvider.Google);
            System.Diagnostics.Debug.WriteLine($"[DroidLoginProvider] UserId = {client.CurrentUser.UserId} Token = {client.CurrentUser.MobileServiceAuthenticationToken}");
        }

        public async Task LogoutAsync(MobileServiceClient client)
        {
            System.Diagnostics.Debug.WriteLine($"[DroidLoginProvider] LogoutAsync");
            await client.LogoutAsync();
        }
    }
}