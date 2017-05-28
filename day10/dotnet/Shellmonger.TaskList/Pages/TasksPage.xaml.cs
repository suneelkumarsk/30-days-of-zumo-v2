using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Shellmonger.TaskList.Services;
using System;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Shellmonger.TaskList.Pages
{
    public sealed partial class TasksPage : Page
    {
        private IMobileServiceTable<TodoItem> dataTable = App.CloudProvider.GetTable<TodoItem>();
        private MobileServiceCollection<TodoItem, TodoItem> tasks;

        public TasksPage()
        {
            this.InitializeComponent();
        }

        private void Trace(string msg)
        {
            System.Diagnostics.Debug.WriteLine($"TasksPage: {msg}");
        }

        /// <summary>
        /// When the page is brought up, refresh the table.
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await RefreshTasks();
        }

        /// <summary>
        /// Event Handler: Log Out
        /// </summary>
        private async void HangUp_Click(object sender, TappedRoutedEventArgs e)
        {
            if (!LogoutIcon.IsTapEnabled) return;
            Trace("HangUp_Click");

            // Change the color of the button to gray and disable it
            var color = LogoutIcon.Foreground;
            LogoutIcon.Foreground = new SolidColorBrush(Colors.Gray);
            LogoutIcon.IsTapEnabled = false;

            try
            {
                await App.CloudProvider.LogoutAsync();
                Frame.Navigate(typeof(EntryPage));
            }
            catch (CloudAuthenticationException exception)
            {
                Trace($"Failed to logout: {exception.InnerException.Message}");
                var dialog = new MessageDialog(exception.Message);
                await dialog.ShowAsync();
            }

            // Re-enable the button
            LogoutIcon.Foreground = color;
            LogoutIcon.IsTapEnabled = true;
        }

        /// <summary>
        /// Event Handler: Clicked on People
        /// </summary>
        private void People_Click(object sender, TappedRoutedEventArgs e)
        {
            Trace("People_Click");
            Frame.Navigate(typeof(PeoplePage));
        }

        /// <summary>
        /// Event Handler: Clicked on Refresh Icon
        /// </summary>
        private async void RefreshIcon_Click(object sender, RoutedEventArgs e)
        {
            if (!RefreshIcon.IsTapEnabled) return;
            Trace("RefreshIcon_Click");
            await RefreshTasks();
        }

        /// <summary>
        /// Event Handler: Clicked on Add Task Icon
        /// </summary>
        private async void AddTaskIcon_Click(object sender, TappedRoutedEventArgs e)
        {
            if (!AddTask.IsTapEnabled) return;
            Trace("AddTaskIcon_Click");

            // Disable the Add Task Icon
            var color = AddTask.Foreground;
            AddTask.Foreground = new SolidColorBrush(Colors.Gray);
            AddTask.IsTapEnabled = false;

            // Build the contents of the dialog box
            var stackPanel = new StackPanel();
            var taskBox = new TextBox
            {
                Text = "",
                PlaceholderText = "Enter New Task",
                Margin = new Windows.UI.Xaml.Thickness(8.0)
            };
            stackPanel.Children.Add(taskBox);

            // Create the dialog box
            var dialog = new ContentDialog
            {
                Title = "Add New Task",
                PrimaryButtonText = "OK",
                SecondaryButtonText = "Cancel"
            };
            dialog.Content = stackPanel;

            // Show the dialog box and handle the response
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var newTask = new TodoItem { Title = taskBox.Text.Trim() };
                Trace("AddTaskIcon_Click: Adding new task to server");
                StartNetworkActivity();
                try
                {
                    await dataTable.InsertAsync(newTask);
                    tasks.Add(newTask);
                    Trace($"AddTaskIcon_Click: New Task ID = {newTask.Id}, Title={newTask.Title}");
                }
                catch (MobileServiceInvalidOperationException exception)
                {
                    var opDialog = new MessageDialog(exception.Message);
                    await opDialog.ShowAsync();
                }
                StopNetworkActivity();
            }

            // Enable the Add Task Icon
            AddTask.Foreground = color;
            AddTask.IsTapEnabled = true;
        }

        /// <summary>
        /// Event Handler: A checkbox on the list was either set or cleared
        /// </summary>
        private async void taskComplete_Changed(object sender, RoutedEventArgs e)
        {
            Trace("taskComplete_Changed");
            CheckBox cb = (CheckBox)sender;
            TodoItem item = cb.DataContext as TodoItem;
            if (item.Completed == (bool)cb.IsChecked)
            {
                Trace($"taskComplete_Changed: Skipping (same data) ID={item.Id}");
                return;
            }
            item.Completed = (bool)cb.IsChecked;

            StartNetworkActivity();
            try
            {
                Trace($"Updating Item ID={item.Id} Title={item.Title} Completed={item.Completed}");
                await dataTable.UpdateAsync(item);
            }
            catch (MobileServiceInvalidOperationException exception)
            {
                Trace($"Error updating item ID={item.Id} Error={exception.Message}");
                var dialog = new MessageDialog(exception.Message);
                await dialog.ShowAsync();
            }
            StopNetworkActivity();

            TaskListView.Focus(FocusState.Unfocused);
        }

        /// <summary>
        /// Event Handler: The text box has been updated
        /// </summary>
        private async void taskTitle_Changed(object sender, TextChangedEventArgs e)
        {
            TextBox taskTitle = (TextBox)sender;
            TodoItem item = taskTitle.DataContext as TodoItem;
            if (item == null) return;

            Trace("taskTitle_Changed");
            if (item.Title.Equals(taskTitle.Text.Trim()))
            {
                Trace("taskTitle_Changed - text is the same");
                return;
            }
            item.Title = taskTitle.Text.Trim();
            StartNetworkActivity();
            try
            {
                Trace($"taskTitle_Changed - updating title ID={item.Id} Title={item.Title} Complete={item.Completed}");
                await dataTable.UpdateAsync(item);
                TaskListView.Focus(FocusState.Unfocused);
            }
            catch (MobileServicePreconditionFailedException<TodoItem> conflict)
            {
                Trace($"taskTitle_Changed - Conflict Resolution for item ${conflict.Item.Id}");

                // If the two versions are the same, then ignore the conflict - client wins
                if (conflict.Item.Title.Equals(item.Title) && conflict.Item.Completed == item.Completed)
                {
                    item.Version = conflict.Item.Version;
                }
                else
                {
                    // Build the contents of the dialog box
                    var stackPanel = new StackPanel();
                    var localVersion = new TextBlock
                    {
                        Text = $"Local Version: Title={item.Title} Completed={item.Completed}"
                    };
                    var serverVersion = new TextBlock
                    {
                        Text = $"Server Version: Title={conflict.Item.Title} Completed={conflict.Item.Completed}"
                    };
                    stackPanel.Children.Add(localVersion);
                    stackPanel.Children.Add(serverVersion);

                    // Create the dialog box
                    var dialog = new ContentDialog
                    {
                        Title = "Resolve Conflict",
                        PrimaryButtonText = "Local",
                        SecondaryButtonText = "Server"
                    };
                    dialog.Content = stackPanel;

                    // Show the dialog box and handle the response
                    var result = await dialog.ShowAsync();
                    if (result == ContentDialogResult.Primary)
                    {
                        // Local Version - Copy the version from server to client and re-submit
                        item.Version = conflict.Item.Version;
                        await dataTable.UpdateAsync(item);
                    }
                    else if (result == ContentDialogResult.Secondary)
                    {
                        // Just pull the records from the server
                        await RefreshTasks();
                    }
                }
            }
            catch (MobileServiceInvalidOperationException exception)
            {
                Trace($"taskTitle_Changed - failed to update title ID={item.Id} Title={item.Title} Error={exception.Message}");
                var dialog = new MessageDialog(exception.Message);
                await dialog.ShowAsync();
            }
            StopNetworkActivity();
            return;
        }


        /// <summary>
        /// Event Handler: Delete a record
        /// </summary>
        private async void taskDelete_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Trace("taskDelete_tapped");
            SymbolIcon icon = (SymbolIcon)sender;
            TodoItem item = icon.DataContext as TodoItem;

            StartNetworkActivity();
            try
            {
                Trace($"Deleting task id={item.Id} title={item.Title} completed={item.Completed}");
                await dataTable.DeleteAsync(item);
                tasks.Remove(item);
            }
            catch (MobileServiceInvalidOperationException exception)
            {
                Trace($"taskDelete_Tapped - failed to delete title ID={item.Id} Error={exception.Message}");
                var dialog = new MessageDialog(exception.Message);
                await dialog.ShowAsync();
            }

            StopNetworkActivity();
        }

        /// <summary>
        /// Refresh the Tasks List
        /// </summary>
        private async Task RefreshTasks()
        {
            StartNetworkActivity();
            try
            {
                Trace("RefreshTasks()");
                tasks = await dataTable.ToCollectionAsync();
                TaskListView.ItemsSource = tasks;
                Trace($"Received {tasks.Count} tasks");
            }
            catch (MobileServiceInvalidOperationException exception)
            {
                Trace($"Error refreshing tasks: {exception.Message}");
                await new MessageDialog(exception.Message, "Error Loading Tasks").ShowAsync();
            }
            StopNetworkActivity();
        }

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

    }
}
