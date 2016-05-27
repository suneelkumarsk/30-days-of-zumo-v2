using Android.App;
using Android.Content.PM;
using Android.OS;
using Acr.UserDialogs;
using XamarinTodo.Droid.Services;
using XamarinTodo.Services;
using Xamarin.Forms;

namespace XamarinTodo.Droid
{
    [Activity(Label = "XamarinTodo.Droid", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Initialization for Azure Mobile Apps
            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
            // Initialization for Acr.UserDialogs
            UserDialogs.Init(this);

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            // Initialize the Droid version of the LoginProvider
            var loginProvider = (DroidLoginProvider)DependencyService.Get<ILoginProvider>();
            loginProvider.Initialize(this);

            // Initialize the Droid version of the FileProvider
            var fileProvider = (DroidFileProvider)DependencyService.Get<IFileProvider>();
            fileProvider.UIContext = this;

            LoadApplication(new App());
        }
    }
}

