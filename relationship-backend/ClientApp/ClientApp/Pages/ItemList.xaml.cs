using Xamarin.Forms;

namespace ClientApp.Pages
{
    public partial class ItemList : ContentPage
    {
        public ItemList()
        {
            InitializeComponent();
            BindingContext = new ViewModels.ItemListViewModel();
        }
    }
}
