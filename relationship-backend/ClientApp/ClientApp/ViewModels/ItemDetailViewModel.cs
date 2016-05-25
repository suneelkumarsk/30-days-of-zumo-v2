using ClientApp.Helpers;
using ClientApp.Models;
using ClientApp.Services;
using Xamarin.Forms;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using System;

namespace ClientApp.ViewModels
{
    public class ItemDetailViewModel : Base
    {
        readonly ICloudService cloudService;
        Command c_save, c_delete;

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

            if ((cloudService != null) && (item.TagId != null))
            {
                Tag = cloudService.GetTagByIdAsync(item.TagId).Result;
            }
        }

        public TodoItem Item { get; set; }
        public Tag Tag { get; set; }

#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
        public Command SaveCommand => c_save ?? (c_save = new Command(async () => await ExecuteSaveCommand()));

        public Command DeleteCommand => c_delete ?? (c_delete = new Command(async () => await ExecuteDeleteCommand()));
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void

        async Task ExecuteSaveCommand()
        {
            if (IsBusy)
                return;
            IsBusy = true;

            try
            {
                UserDialogs.Instance.ShowLoading("Saving Item");
                await cloudService.UpsertItemAsync(Item);
                UserDialogs.Instance.HideLoading();
                MessagingCenter.Send<ItemDetailViewModel>(this, "ItemsChanged");
                await Application.Current.MainPage.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading();
                UserDialogs.Instance.ShowError(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task ExecuteDeleteCommand()
        {
            if (IsBusy)
                return;
            IsBusy = true;

            try
            {
                UserDialogs.Instance.ShowLoading("Deleting Item");
                await cloudService.DeleteItemAsync(Item);
                UserDialogs.Instance.HideLoading();
                MessagingCenter.Send<ItemDetailViewModel>(this, "ItemsChanged");
                await Application.Current.MainPage.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading();
                UserDialogs.Instance.ShowError(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
