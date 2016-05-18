using Xamarin.Forms;
using XamarinTodo.Helpers;
using XamarinTodo.Services;

namespace XamarinTodo
{
    public partial class App : Application
    {
        static readonly bool useMockServices = false;

        public App()
        {
            InitializeComponent();

            if (useMockServices)
                ServiceLocator.Instance.Add<ICloudService, MockCloudService>();
            else
                ServiceLocator.Instance.Add<ICloudService, AzureCloudService>();

            MainPage = new NavigationPage(new Pages.EntryPage());
        }
    }
}
