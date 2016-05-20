using Acr.UserDialogs;
using Plugin.Media;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using XamarinTodo.Helpers;
using XamarinTodo.Models;
using XamarinTodo.Services;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace XamarinTodo.ViewModels
{
    public class ItemListViewModel : ViewModels.Base
    {
        ICloudService cloudService;

        public ItemListViewModel()
        {
            Title = "Xamarin Task List";
            cloudService = ServiceLocator.Instance.Resolve<ICloudService>();

#pragma warning disable CS4014
            // The ItemListViewModel is not async aware, Refresh happens async anyway
            RefreshList();
#pragma warning restore CS4014

            items.CollectionChanged += this.OnCollectionChanged;
        }

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Debug.WriteLine("[ItemListViewModel] OnCollectionChanged:Items has changed");
        }

        #region Properties
        ObservableCollection<TodoItem> items = new ObservableCollection<TodoItem>();
        public ObservableCollection<TodoItem> Items
        {
            get { return items; }
            set { SetProperty(ref items, value, "Items"); }
        }

        TodoItem selectedItem;
        public TodoItem SelectedItem
        {
            get { return selectedItem; }
            set
            {
                SetProperty(ref selectedItem, value, "SelectedItem");
                if (selectedItem != null)
                {
                    var nav = Application.Current.MainPage as NavigationPage;
                    nav.PushAsync(new Pages.ItemDetail(selectedItem));
                    SelectedItem = null;
                }
            }
        }
        #endregion

        #region Commands
        Command c_refresh;
        public Command RefreshCommand
        {
            get { return c_refresh ?? (c_refresh = new Command(async () => await ExecuteRefreshCommand())); }
        }

        async Task ExecuteRefreshCommand()
        {
            if (IsBusy)
                return;
            IsBusy = true;

            try
            {
                UserDialogs.Instance.ShowLoading("Loading Items...");
                var list = await cloudService.GetAllItemsAsync();
                Items.Clear();
                foreach (var item in list)
                {
                    Items.Add(item);
                }
                UserDialogs.Instance.HideLoading();
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

        Command c_addNew;
        public Command AddNewItemCommand
        {
            get { return c_addNew ?? (c_addNew = new Command(async () => await ExecuteAddNewItemCommand())); }
        }

        async Task ExecuteAddNewItemCommand()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                await Application.Current.MainPage.Navigation.PushAsync(new Pages.ItemDetail());
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

        Command c_uploadFile;
        public Command UploadFileCommand
        {
            get { return c_uploadFile ?? (c_uploadFile = new Command(async () => await ExecuteUploadFileCommand())); }
        }

        async Task ExecuteUploadFileCommand()
        {
            if (IsBusy) return;
            IsBusy = true;

            Uri storageUri = null;
            try
            {
                // Xamarin Media Plugin to get a picture
                await CrossMedia.Current.Initialize();
                var file = await CrossMedia.Current.PickPhotoAsync();

                // Get the storage token from the custom API
                var storageToken = await cloudService.GetStorageToken();
                storageUri = new Uri($"{storageToken.Uri}{storageToken.SasToken}");

                // Store the MediaFile to the storage token
                var blob = new CloudBlockBlob(storageUri);
                var stream = file.GetStream();
                await blob.UploadFromStreamAsync(stream);
                UserDialogs.Instance.SuccessToast("File Upload Complete", null, 1500);
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert(storageUri.ToString(), ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
        #endregion

        async Task RefreshList()
        {
            await ExecuteRefreshCommand();
            MessagingCenter.Subscribe<ItemDetailViewModel>(this, "ItemsChanged", async (sender) =>
            {
                Debug.WriteLine("[ItemListViewModel] Received ItemsChanged event from ItemDetailViewModel");
                await ExecuteRefreshCommand();
            });
        }
    }
}
