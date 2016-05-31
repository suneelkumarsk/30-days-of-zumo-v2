using Acr.UserDialogs;
using Microsoft.WindowsAzure.MobileServices.Files;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

			// Load the Images async
			Images = new ObservableCollection<TodoItemImage>();
#pragma warning disable CS4014 // Statement is not awaited
            LoadImagesAsync();
#pragma warning restore CS4014 // Statement is not awaited
        }

        public TodoItem Item { get; set; }
        public ObservableCollection<TodoItemImage> Images { get; set; }

        public async Task LoadImagesAsync()
        {
            IEnumerable<MobileServiceFile> files = await cloudService.GetItemImageFilesAsync(Item);
            Images.Clear();
            foreach (var file in files)
            {
                Images.Add(new TodoItemImage(file, Item));
            }
        }

        #region Commands
        Command c_save;
        public Command SaveCommand => c_save ?? (c_save = new Command(async () => ExecuteSaveCommand()));

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
        public Command DeleteCommand => c_delete ?? (c_delete = new Command(async () => ExecuteDeleteCommand()));

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

        Command c_addimage;
        public Command AddImageCommand => c_addimage ?? (c_addimage = new Command(async () => ExecuteAddImageCommand()));

        async Task ExecuteAddImageCommand()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var image = await DependencyService.Get<IFileProvider>().GetImageAsync();
                if (image != null)
                {
                    var file = await cloudService.AddItemImageAsync(Item, image);
                    var attachedImage = new TodoItemImage(file, Item);
                    Images.Add(attachedImage);
                }
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
