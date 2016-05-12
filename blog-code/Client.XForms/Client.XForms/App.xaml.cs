using Client.XForms.Helpers;
using Client.XForms.Services;
using Xamarin.Forms;

namespace Client.XForms
{
    public partial class App : Application
    {
        bool useMockService = true;

        public App()
        {
            InitializeComponent();

            if (useMockService)
            {
                ServiceLocator.Instance.Add<ICloudService, Mockservice>();
            }
            //else
            //{
            //    ServiceLocator.Instance.Add<ICloudService, AzureService>();
            //}

            MainPage = new NavigationPage(new Pages.EntryPage());
        }
    }
}
