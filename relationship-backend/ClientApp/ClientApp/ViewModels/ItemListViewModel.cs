using Acr.UserDialogs;
using ClientApp.Helpers;
using ClientApp.Models;
using ClientApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ClientApp.ViewModels
{
    public class ItemListViewModel : ViewModels.Base
    {
        ICloudService cloudService;
        ObservableCollection<TodoItem> items = new ObservableCollection<TodoItem>();
        ObservableCollection<Tag> tags = new ObservableCollection<Tag>();
        TodoItem selectedItem;

        public ItemListViewModel()
        {
            Title = "ClientApp Task List";
            cloudService = ServiceLocator.Instance.Resolve<ICloudService>();
#pragma warning disable CS4014
            RefreshList();
#pragma warning restore CS4014
            items.CollectionChanged += this.OnCollectionChanged;
        }

        public ObservableCollection<TodoItem> Items
        {
            get
            {
                return items;
            }

            set
            {
                SetProperty(ref items, value, "Items");
            }
        }

        public ObservableCollection<Tag> Tags
        {
            get
            {
                return tags;
            }

            set
            {
                SetProperty(ref tags, value, "Tags");
            }
        }

        public TodoItem SelectedItem
        {
            get
            {
                return selectedItem;
            }

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

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Debug.WriteLine("[ItemListViewModel] OnCollectionChanged");
        }

        Command c_refresh, c_addNew;

        public Command RefreshCommand
        {
            get
            {
                return c_refresh ?? (c_refresh = new Command(async () => await ExecuteRefreshCommand()));
            }
        }

        public Command AddNewItemCommand
        {
            get
            {
                return c_addNew ?? (c_addNew = new Command(async () => await ExecuteAddNewItemCommand()));
            }
        }

        async Task ExecuteRefreshCommand()
        {
            if (IsBusy)
                return;
            IsBusy = true;

            try
            {
                UserDialogs.Instance.ShowLoading("Loading...");
                var itemlist = await cloudService.GetAllItemsAsync();
                Items.Clear();
                foreach (var item in itemlist)
                {
                    Items.Add(item);
                }

                var taglist = await cloudService.GetAllTagsAsync();
                Tags.Clear();
                foreach (var tag in taglist)
                {
                    Tags.Add(tag);
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

        async Task ExecuteAddNewItemCommand()
        {
            if (IsBusy)
                return;
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

        async Task RefreshList()
        {
            await ExecuteRefreshCommand();
            MessagingCenter.Subscribe<ItemDetailViewModel>(this, "ItemsChanged", async (sender) =>
            {
                Debug.WriteLine("[ItemListViewModel] ItemsChanged Event received");
                await ExecuteRefreshCommand();
            });
        }
    }
}
