using Xamarin.Forms;
using ClientApp.Helpers;
using ClientApp.Services;

namespace ClientApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            ServiceLocator.Instance.Add<ICloudService, AzureCloudService>();
            MainPage = new NavigationPage(new Pages.TodoList());
        }
    }
}
