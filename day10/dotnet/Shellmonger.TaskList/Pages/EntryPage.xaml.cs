using Shellmonger.TaskList.Services;
using System;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Shellmonger.TaskList.Pages
{
    public sealed partial class EntryPage : Page
    {
        public EntryPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Event Handler for the Entry Button
        /// </summary>
        private async void EntryButton_Clicked(object sender, TappedRoutedEventArgs e)
        {
            // Change the color of the button to gray and disable it
            var color = EntryButton.Background;
            EntryButton.Background = new SolidColorBrush(Colors.Gray);
            EntryButton.IsTapEnabled = false;

            try
            {
                await App.CloudProvider.LoginAsync();
                Frame.Navigate(typeof(TasksPage));
            }
            catch (CloudAuthenticationException exception)
            {
                System.Diagnostics.Debug.WriteLine("Failed to authenticate: {0}", exception.InnerException.Message);
                var dialog = new MessageDialog(exception.Message);
                await dialog.ShowAsync();
            }

            // Re-enable the button
            EntryButton.Background = color;
            EntryButton.IsTapEnabled = true;
        }
    }
}
