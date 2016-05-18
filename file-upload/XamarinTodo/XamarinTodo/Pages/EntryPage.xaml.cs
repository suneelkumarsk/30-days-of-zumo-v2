using Xamarin.Forms;
using XamarinTodo.ViewModels;

namespace XamarinTodo.Pages
{
    public partial class EntryPage : ContentPage
    {
        public EntryPage()
        {
            InitializeComponent();

            BindingContext = new EntryPageViewModel();
        }
    }
}
