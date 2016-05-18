using Acr.UserDialogs;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using XamarinTodo.Helpers;
using XamarinTodo.Models;
using XamarinTodo.Services;

namespace XamarinTodo.ViewModels
{
    public class ItemDetailViewModel : Base
    {
        ICloudService cloudService;

        public ItemDetailViewModel(TodoItem item = null)
        {
            if (item != null)
            {
                Item = item;
                Title = item.Text;
            }
            else
            {
                Item = new TodoItem { Text = "New Item", Complete = false };
                Title = "New Item";
            }
            cloudService = ServiceLocator.Instance.Resolve<ICloudService>();
        }

        public TodoItem Item { get; set; }

        #region Commands
        Command c_save;
        public Command SaveCommand
        {
            get { return c_save ?? (c_save = new Command(async () => ExecuteSaveCommand())); }
        }

        async Task ExecuteSaveCommand()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                Debug.WriteLine("[ItemDetailViewModel] Saving Item");
                UserDialogs.Instance.ShowLoading("Saving Item");
                await cloudService.UpsertItemAsync(Item);
                Debug.WriteLine("[ItemDetailViewModel] Saved Item");
                UserDialogs.Instance.HideLoading();
                Debug.WriteLine("[ItemDetailViewModel] Sending ItemsChanged to MessagingCenter");
                MessagingCenter.Send<ItemDetailViewModel>(this, "ItemsChanged");
                var nav = Application.Current.MainPage.Navigation.PopAsync(); // Pop Back
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

        Command c_delete;
        public Command DeleteCommand
        {
            get { return c_delete ?? (c_delete = new Command(async () => ExecuteDeleteCommand())); }
        }

        async Task ExecuteDeleteCommand()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                UserDialogs.Instance.ShowLoading("Deleting Item");
                await cloudService.DeleteItemAsync(Item);
                UserDialogs.Instance.HideLoading();
                MessagingCenter.Send<ItemDetailViewModel>(this, "ItemsChanged");
                var nav = Application.Current.MainPage.Navigation.PopAsync(); // Pop Back
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
