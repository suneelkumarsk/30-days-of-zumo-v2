using Client.UWP.Models;
using Client.UWP.Services;
using System;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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
            StartNetworkActivity();
            TaskListView.ItemsSource = await dataTable.RefreshAsync();
            StopNetworkActivity();
        }

        #region HangUp Clicked Event Handler
        private async void HangUp_Click(object sender, TappedRoutedEventArgs e)
        {
            if (!LogoutIcon.IsTapEnabled) return;

            var color = LogoutIcon.Foreground;
            LogoutIcon.Foreground = new SolidColorBrush(Colors.Gray);
            LogoutIcon.IsTapEnabled = false;

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

            LogoutIcon.Foreground = color;
            LogoutIcon.IsTapEnabled = true;
        }

        private async void RefreshIcon_Click(object sender, RoutedEventArgs e)
        {
            if (!RefreshIcon.IsTapEnabled) return;

            StartNetworkActivity();
            TaskListView.ItemsSource = await dataTable.RefreshAsync();
            StopNetworkActivity();
        }
        #endregion

        #region Add Task Event Handler
        private async void AddTaskIcon_Click(object sender, TappedRoutedEventArgs e)
        {
            if (!AddTask.IsTapEnabled) return;

            var color = AddTask.Foreground;
            AddTask.Foreground = new SolidColorBrush(Colors.Gray);
            AddTask.IsTapEnabled = false;

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
                var newTask = new TodoItem { Text = taskBox.Text.Trim() };
                StartNetworkActivity();
                try
                {
                    await dataTable.SaveAsync(newTask);
                }
                catch (CloudTableOperationFailed exception)
                {
                    await ShowDialog("Save Failed", exception.Message);
                }
                StopNetworkActivity();
            }

            AddTask.Foreground = color;
            AddTask.IsTapEnabled = true;
        }
        #endregion

        #region List Editor Event Handlers
        private async void taskComplete_Changed(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            TodoItem item = cb.DataContext as TodoItem;

            if (item.Completed == (bool)cb.IsChecked)
            {
                return;
            }

            item.Completed = (bool)cb.IsChecked;

            StartNetworkActivity();
            try
            {
                await dataTable.SaveAsync(item);
            }
            catch (CloudTableOperationFailed exception)
            {
                await ShowDialog("Save Failed", exception.Message);
            }
            StopNetworkActivity();
            TaskListView.Focus(FocusState.Unfocused);
        }

        private async void taskTitle_Changed(object sender, TextChangedEventArgs e)
        {
            TextBox taskTitle = (TextBox)sender;
            TodoItem item = taskTitle.DataContext as TodoItem;
            if (item == null || item.Text.Equals(taskTitle.Text.Trim())) return;

            item.Text = taskTitle.Text.Trim();
            StartNetworkActivity();
            try
            {
                await dataTable.SaveAsync(item);
            }
            catch (CloudTableOperationFailed exception)
            {
                await ShowDialog("Save Failed", exception.Message);
            }
            StopNetworkActivity();
            TaskListView.Focus(FocusState.Unfocused);
        }

        private async void taskDelete_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SymbolIcon icon = (SymbolIcon)sender;
            TodoItem item = icon.DataContext as TodoItem;

            StartNetworkActivity();
            try
            {
                await dataTable.DeleteAsync(item);
            }
            catch (CloudTableOperationFailed exception)
            {
                await ShowDialog("Delete Failed", exception.Message);
            }
            StopNetworkActivity();
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

        #region Network Activity Indicators
        /// <summary>
        /// Start rotating the refresh icon
        /// </summary>
        private void StartNetworkActivity()
        {
            RefreshIconRotation.Begin();
            RefreshIcon.IsTapEnabled = false;
        }

        /// <summary>
        /// Stop rotating the refresh icon
        /// </summary>
        private void StopNetworkActivity()
        {
            RefreshIcon.IsTapEnabled = true;
            RefreshIconRotation.Stop();
        }
        #endregion
    }
}
