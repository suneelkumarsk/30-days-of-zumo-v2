using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
