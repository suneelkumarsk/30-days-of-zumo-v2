using Client.XForms.ViewModels;
using Xamarin.Forms;

namespace Client.XForms.Pages
{
    public partial class TodoList : ContentPage
    {
        TodoListViewModel viewModel;

        public TodoList()
        {
            InitializeComponent();
            viewModel = new TodoListViewModel();
            BindingContext = viewModel;
        }
    }
}
