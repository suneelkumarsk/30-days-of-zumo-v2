using Client.XForms.Services;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Client.XForms.Pages
{
    public partial class EntryPage : ContentPage
    {
        ICloudService cloudService;

        public EntryPage()
        {
            InitializeComponent();
            cloudService = ServiceLocator.Instance.Resolve<ICloudService>();
        }

        #region Login Button
        Command loginCommand;
        public Command LoginCommand
        {
            get { return loginCommand ?? (loginCommand = new Command(async () => await ExecuteLoginCommand())); }
        }

        async Task ExecuteLoginCommand()
        {
            if (IsBusy)
                return;
            IsBusy = true;

            try
            {
                await cloudService.LoginAsync();
                await Application.Current.MainPage.Navigation.PushAsync(new Pages.TodoList());
            }
            catch (CloudAuthenticationException ex)
            {
                await DisplayAlert("Exception", ex.Message, "Dismiss");
            }
            finally
            {
                IsBusy = false;
            }
        }
        #endregion
    }
}
