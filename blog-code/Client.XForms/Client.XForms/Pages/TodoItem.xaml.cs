using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Client.XForms.Pages
{
    public partial class TodoItem : ContentPage
    {
        public TodoItem(Models.TodoItem item = null)
        {
            InitializeComponent();
            if (item != null)
                BindingContext = new ViewModels.TodoItemViewModel(item);
            else
                BindingContext = new ViewModels.TodoItemViewModel();
        }
    }
}
