using ClientApp.Models;
using Xamarin.Forms;

namespace ClientApp.Pages
{
    public partial class ItemDetail : ContentPage
    {
        public ItemDetail(TodoItem item = null)
        {
            InitializeComponent();
            BindingContext = new ViewModels.ItemDetailViewModel(item);
        }
    }
}
