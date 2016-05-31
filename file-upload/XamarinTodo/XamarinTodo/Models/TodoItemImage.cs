using Microsoft.WindowsAzure.MobileServices.Files;
using System.ComponentModel;
using XamarinTodo.Services;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Diagnostics;

namespace XamarinTodo.Models
{
    public class TodoItemImage : INotifyPropertyChanged
    {
		private IFileProvider fileProvider;
        private string name, uri;
        private TodoItem todoitem;

        public TodoItemImage(MobileServiceFile file, TodoItem item)
        {
            Name = file.Name;
            File = file;
            todoitem = item;
            fileProvider = DependencyService.Get<IFileProvider>();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            InitializeUriAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
        private async Task InitializeUriAsync()
        {
            Uri = await fileProvider.GetLocalFilePathAsync(todoitem.Id, Name);
        }
        public MobileServiceFile File { get; }

        public string Name
        {
            get { return name; }
            set { name = value;  OnPropertyChanged(nameof(Name)); }
        }

        public string Uri
        {
            get { return uri; }
            set { uri = value;  OnPropertyChanged(nameof(Uri)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
