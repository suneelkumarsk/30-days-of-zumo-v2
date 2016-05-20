using Xamarin.Forms;
using XamarinTodo.Helpers;
using XamarinTodo.Services;

namespace XamarinTodo.Pages
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
