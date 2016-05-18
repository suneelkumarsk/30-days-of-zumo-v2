using Acr.UserDialogs;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using XamarinTodo.Helpers;
using XamarinTodo.Models;
using XamarinTodo.Services;

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
                var list = await cloudService.GetAllItemsAsync();
                Items.Clear();
                foreach (var item in list)
                {
                    Items.Add(item);
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
