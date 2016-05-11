using Client.XForms.Models;
using Client.XForms.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Client.XForms.ViewModels
{
    public class TodoListViewModel : ViewModelBase
    {
        ICloudService cloudService;

        public TodoListViewModel()
        {
            Title = "Todo List";
            cloudService = ServiceLocator.Instance.Resolve<ICloudService>();

            // Refresh the list
            Refresh();
        }

        private async void Refresh()
        {
            await ExecuteRefreshCommand();
            MessagingCenter.Subscribe<TodoItemViewModel>(this, "ItemsChanged", async (sender) =>
            {
                await ExecuteRefreshCommand();
            });
        }

        #region Properties
        #region Bindable Property: TodoItems
        private ObservableCollection<TodoItem> _items;
        public ObservableCollection<TodoItem> TodoItems
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged("TodoItems");
            }
        }
        #endregion

        #region Bindable Property: SelectedItem
        private TodoItem _selectedItem;
        public TodoItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
                // Navigate to details page
                if (_selectedItem != null)
                {
                    var nav = Application.Current.MainPage as NavigationPage;
                    nav.PushAsync(new Pages.TodoItem(SelectedItem));
                    SelectedItem = null;
                }
            }
        }
        #endregion
        #endregion

        #region Commands
        #region Refresh Command
        Command _cmdRefresh;
        public Command RefreshCommand
        {
            get
            {
                return _cmdRefresh ?? (_cmdRefresh = new Command(async () => await ExecuteRefreshCommand()));
            }
        }
        async Task ExecuteRefreshCommand()
        {
            if (IsBusy)
                return;
            IsBusy = true;

            try
            {
                var items = await cloudService.GetItemsAsync();
                TodoItems.Clear();
                foreach (var item in items)
                {
                    TodoItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Exception", ex.Message, "Dismiss");
            }
            finally
            {
                IsBusy = false;
            }
        }
        #endregion

        #region Refresh Command
        Command _cmdAddNew;
        public Command AddNewItemCommand
        {
            get
            {
                return _cmdAddNew ?? (_cmdAddNew = new Command(async () => await ExecuteAddNewItemCommand()));
            }
        }
        async Task ExecuteAddNewItemCommand()
        {
            if (IsBusy)
                return;
            IsBusy = true;

            try
            {
                await Application.Current.MainPage.Navigation.PushAsync(new Pages.TodoItem());
            }
            catch (Exception ex)
            {
                DisplayAlert("Exception", ex.Message, "Dismiss");
            }
            finally
            {
                IsBusy = false;
            }
        }
        #endregion
        #endregion
    }
}