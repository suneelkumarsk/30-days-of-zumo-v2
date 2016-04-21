using Client.UWP.Controls;
using Client.UWP.Models;
using Client.UWP.Services;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


namespace Client.UWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TasksPage : Page
    {
        private AzureDataTable<TodoItem> dataTable = AzureCloudProvider.Instance.GetTable<TodoItem>();

        public TasksPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            RefreshIcon.IsEnabled = false;
            TaskListView.ItemsSource = await dataTable.RefreshAsync();
            RefreshIcon.IsEnabled = true;
        }

        #region HangUp Clicked Event Handler
        private async void HangUp_Click(object sender, RoutedEventArgs e)
        {
            LogoutIcon.IsEnabled = false;
            try
            {
                await AzureCloudProvider.Instance.LogoutAsync();
                Frame.Navigate(typeof(EntryPage));
            }
            catch (CloudAuthenticationFailedException exception)
            {
                var dialog = new MessageDialog(exception.Message, "Logout Failed");
                await dialog.ShowAsync();
            }
            LogoutIcon.IsEnabled = true;
        }

        private async void RefreshIcon_Click(object sender, RoutedEventArgs e)
        {
            RefreshIcon.IsEnabled = false;
            TaskListView.ItemsSource = await dataTable.RefreshAsync();
            RefreshIcon.IsEnabled = true;
        }
        #endregion

        #region Add Task Event Handler
        private async void AddTask_Click(object sender, RoutedEventArgs e)
        {
            AddTask.IsEnabled = false;

            var taskBox = new TextBox
            {
                Text = "",
                PlaceholderText = "Enter New Task",
                Margin = new Thickness(8.0)
            };

            var stackPanel = new StackPanel();
            stackPanel.Children.Add(taskBox);

            var dialog = new ContentDialog
            {
                Title = "Add New Task",
                PrimaryButtonText = "OK",
                SecondaryButtonText = "Cancel",
                Content = stackPanel
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var newTask = new TodoItem { Title = taskBox.Text.Trim() };
                RefreshIcon.IsEnabled = false;
                try
                {
                    await dataTable.SaveAsync(newTask);
                }
                catch (CloudTableOperationFailed exception)
                {
                    await ShowDialog("Save Failed", exception.Message);
                }
                RefreshIcon.IsEnabled = true;
            }

            AddTask.IsEnabled = true;
        }
        #endregion

        #region List Editor Event Handlers
        private async void taskComplete_Changed(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            TodoItem item = cb.DataContext as TodoItem;

            if (item.Complete == cb.IsChecked)
            {
                return;
            }
            item.Complete = (bool)cb.IsChecked;

            RefreshIcon.IsEnabled = false;
            try
            {
                await dataTable.SaveAsync(item);
            }
            catch (CloudTableOperationFailed exception)
            {
                await ShowDialog("Save Failed", exception.Message);
            }
            RefreshIcon.IsEnabled = true;
            TaskListView.Focus(FocusState.Unfocused);
        }

        private async void taskTitle_Changed(object sender, TextChangedEventArgs e)
        {
            TextBox taskTitle = (TextBox)sender;
            TodoItem item = taskTitle.DataContext as TodoItem;
            if (item == null || item.Title.Equals(taskTitle.Text.Trim())) return;

            item.Title = taskTitle.Text.Trim();
            RefreshIcon.IsEnabled = false;
            try
            {
                await dataTable.SaveAsync(item);
            }
            catch (CloudTableOperationFailed exception)
            {
                await ShowDialog("Save Failed", exception.Message);
            }
            RefreshIcon.IsEnabled = true;
            TaskListView.Focus(FocusState.Unfocused);
        }
        #endregion

        #region Delete Task Event Handler
        private async void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            TodoItem item = ((FrameworkElement)sender).DataContext as TodoItem;

            RefreshIcon.IsEnabled = false;
            try
            {
                await dataTable.DeleteAsync(item);
            }
            catch (CloudTableOperationFailed exception)
            {
                await ShowDialog("Delete Failed", exception.Message);
            }
            RefreshIcon.IsEnabled = true;
        }
        #endregion

        #region Dialog
        /// <summary>
        /// Displays a dialog with a title, message and OK button
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task ShowDialog(string title, string message)
        {
            var dialog = new MessageDialog(message, title);
            await dialog.ShowAsync();
        }
        #endregion
    }
}
