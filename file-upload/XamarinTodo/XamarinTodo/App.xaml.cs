using Xamarin.Forms;
using XamarinTodo.Helpers;
using XamarinTodo.Services;

namespace XamarinTodo
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            ServiceLocator.Instance.Add<ICloudService, AzureCloudService>();

            MainPage = new NavigationPage(new Pages.EntryPage());
        }
    }
}
