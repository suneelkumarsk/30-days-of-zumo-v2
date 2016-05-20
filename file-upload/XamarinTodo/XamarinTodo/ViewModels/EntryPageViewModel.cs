using Acr.UserDialogs;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using XamarinTodo.Helpers;
using XamarinTodo.Services;

namespace XamarinTodo.ViewModels
{
    public class EntryPageViewModel : ViewModels.Base
    {
        readonly ICloudService cloudService;

        public EntryPageViewModel()
        {
            Title = "Xamarin Todo";
            cloudService = ServiceLocator.Instance.Resolve<ICloudService>();
        }

        #region Commands
        Command c_login;
        public Command LoginCommand => c_login ?? (c_login = new Command(async () => await ExecuteLoginCommand()));

        public async Task ExecuteLoginCommand()
        {
            if (IsBusy)
                return;
            IsBusy = true;

            try
            {
                Debug.WriteLine("[ExecuteLoginCommand] Calling LoginAsync()");
                await cloudService.LoginAsync();
                Debug.WriteLine("[ExecuteLoginCommand] Finished LoginAsync()");
                Application.Current.MainPage = new NavigationPage(new Pages.ItemList());
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.ShowError(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }
        #endregion
    }
}
