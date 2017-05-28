using Microsoft.WindowsAzure.MobileServices;
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
    public sealed partial class PeoplePage : Page
    {
        private IMobileServiceTable<Friend> dataTable = App.CloudProvider.GetTable<Friend>();
        private MobileServiceCollection<Friend, Friend> friends;

        public PeoplePage()
        {
            this.InitializeComponent();
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
                System.Diagnostics.Debug.WriteLine("Failed to logout: {0}", exception.InnerException.Message);
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
        private void Tasks_Click(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(TasksPage));
        }

        private async void RefreshIcon_Click(object sender, RoutedEventArgs e)
        {
            if (!RefreshIcon.IsTapEnabled) return;
            await RefreshTasks();
        }

        private async void AddFriendIcon_Click(object sender, TappedRoutedEventArgs e)
        {
            if (!AddFriendIcon.IsTapEnabled) return;

            // Disable the Add Task Icon
            var color = AddFriendIcon.Foreground;
            AddFriendIcon.Foreground = new SolidColorBrush(Colors.Gray);
            AddFriendIcon.IsTapEnabled = false;

            // Build the contents of the dialog box
            var stackPanel = new StackPanel();
            var friendBox = new TextBox
            {
                Text = "",
                PlaceholderText = "Enter Email Address of Friend",
                Margin = new Windows.UI.Xaml.Thickness(8.0)
            };
            stackPanel.Children.Add(friendBox);

            // Create the dialog box
            var dialog = new ContentDialog
            {
                Title = "Add New Friend",
                PrimaryButtonText = "OK",
                SecondaryButtonText = "Cancel"
            };
            dialog.Content = stackPanel;

            // Show the dialog box and handle the response
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var item = new Friend { Viewer = friendBox.Text.Trim() };
                StartNetworkActivity();
                try
                {
                    await dataTable.InsertAsync(item);
                    friends.Add(item);
                }
                catch (MobileServiceInvalidOperationException exception)
                {
                    var opDialog = new MessageDialog(exception.Message);
                    await opDialog.ShowAsync();
                }
                StopNetworkActivity();
            }

            // Enable the Add Task Icon
            AddFriendIcon.Foreground = color;
            AddFriendIcon.IsTapEnabled = true;
        }

        /// <summary>
        /// Event Handler when a user types something in a text box
        /// </summary>
        private async void nameField_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            Friend item = t.DataContext as Friend;

            // if ESC is pressed, restore the old value
            if (e.Key == Windows.System.VirtualKey.Escape)
            {
                t.Text = item.Viewer;
                t.Focus(FocusState.Unfocused);
                return;
            }

            // if Enter is pressed, store the new value
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                item.Viewer = t.Text;
                StartNetworkActivity();
                try
                {
                    await dataTable.UpdateAsync(item);
                    PeopleListView.Focus(FocusState.Unfocused);
                }
                catch (MobileServiceInvalidOperationException exception)
                {
                    var dialog = new MessageDialog(exception.Message);
                    await dialog.ShowAsync();
                }
                StopNetworkActivity();
                return;
            }
        }

        /// <summary>
        /// Event Handler: Delete a record
        /// </summary>
        private async void nameField_Delete(object sender, TappedRoutedEventArgs e)
        {
            SymbolIcon icon = (SymbolIcon)sender;
            Friend item = icon.DataContext as Friend;

            StartNetworkActivity();
            try
            {
                await dataTable.DeleteAsync(item);
                friends.Remove(item);
            }
            catch (MobileServiceInvalidOperationException exception)
            {
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
                friends = await dataTable.ToCollectionAsync();
                PeopleListView.ItemsSource = friends;
            }
            catch (MobileServiceInvalidOperationException exception)
            {
                await new MessageDialog(exception.Message, "Error Loading Friends").ShowAsync();
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
