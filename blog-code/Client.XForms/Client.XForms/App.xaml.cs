using Client.XForms.Services;
using Xamarin.Forms;

namespace Client.XForms
{
    public partial class App : Application
    {
        public readonly bool useMockService = true;

        public App()
        {
            InitializeComponent();

            if (useMockService)
            {
                ServiceLocator.Instance.Add<ICloudService, MockCloudService>();
            }
            else
            {
                ServiceLocator.Instance.Add<ICloudService, AzureCloudService>();
            }


            // The root page of your application
            MainPage = new NavigationPage(new Pages.EntryPage())
            {
                BarBackgroundColor = (Color)Current.Resources["primaryBlue"],
                BarTextColor = Color.White
            };
        }
    }
}
