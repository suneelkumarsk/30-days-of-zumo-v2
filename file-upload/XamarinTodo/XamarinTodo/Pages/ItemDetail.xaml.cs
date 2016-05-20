using Xamarin.Forms;
using XamarinTodo.Models;

namespace XamarinTodo.Pages
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
