using Client.XForms.Models;
using Client.XForms.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Client.XForms.ViewModels
{
    public class TodoItemViewModel : ViewModelBase
    {
        ICloudService cloudService;
        TodoItem item;

        public TodoItemViewModel(Models.TodoItem item = null)
        {
            if (item != null)
            {
                Title = item.Title;
                this.item = item;
                Text = item.Title;
                Complete = item.Complete;
            }
            else
            {
                Title = "New Todo Item";
            }
            cloudService = ServiceLocator.Instance.Resolve<ICloudService>();
        }

        public string Text { get; set; }
        public bool Complete { get; set; }

        #region Save Item Command
        Command _cmdSave;
        public Command SaveItemCommand
        {
            get
            {
                return _cmdSave ?? (_cmdSave = new Command(async () => await ExecuteSaveItemCommand()));
            }
        }

        async Task ExecuteSaveItemCommand()
        {
            if (IsBusy)
                return;
            IsBusy = true;

            if (item != null)
            {
                item.Title = Text;
                item.Complete = Complete;
            }
            else
            {
                item = new Models.TodoItem
                {
                    Title = Text,
                    Complete = Complete
                };
            }

            try
            {
                await cloudService.AddOrUpdateItemAsync(item);
                MessagingCenter.Send<TodoItemViewModel>(this, "ItemsChanged");
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

        #region Delete Item Command
        Command _cmdDelete;
        public Command DeleteItemCommand
        {
            get
            {
                return _cmdDelete ?? (_cmdDelete = new Command(async () => await ExecuteDeleteItemCommand()));
            }
        }

        async Task ExecuteDeleteItemCommand()
        {
            if (IsBusy)
                return;
            IsBusy = true;

            try
            {
                if (item != null)
                {
                    await cloudService.DeleteItemAsync(item);
                }
                MessagingCenter.Send<TodoItemViewModel>(this, "ItemsChanged");
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

    }
}