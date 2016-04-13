using Client.UWP.Services;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Client.UWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EntryPage : Page
    {
        public EntryPage()
        {
            this.InitializeComponent();
        }

        private async void EntryButton_Clicked(object sender, TappedRoutedEventArgs e)
        {
            var color = EntryButton.Background;
            EntryButton.Background = new SolidColorBrush(Colors.Gray);
            EntryButton.IsTapEnabled = false;

            try
            {
                await AzureCloudProvider.Instance.LoginAsync();
                Frame.Navigate(typeof(TasksPage));
            }
            catch (CloudAuthenticationFailedException loginFailed)
            {
                Debug.WriteLine($"Authentication Failed: {loginFailed.Message}");
                var dialog = new MessageDialog(loginFailed.Message, "Login Failed");
                await dialog.ShowAsync();
            }

            EntryButton.Background = color;
            EntryButton.IsTapEnabled = true;
        }
    }
}
