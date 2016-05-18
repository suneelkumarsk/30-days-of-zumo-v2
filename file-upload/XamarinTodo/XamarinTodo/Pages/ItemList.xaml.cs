using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using XamarinTodo.Models;

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
