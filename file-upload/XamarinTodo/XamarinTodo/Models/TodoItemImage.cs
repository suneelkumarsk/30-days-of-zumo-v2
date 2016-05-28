using Microsoft.WindowsAzure.MobileServices.Files;
using System.ComponentModel;
using XamarinTodo.Services;
using Xamarin.Forms;

namespace XamarinTodo.Models
{
    public class TodoItemImage : INotifyPropertyChanged
    {
		private IFileProvider fileProvider;
        private string name, uri;

        public TodoItemImage(MobileServiceFile file, TodoItem item)
        {
            Name = file.Name;
            File = file;
			fileProvider = DependencyService.Get<IFileProvider>();
            fileProvider.GetLocalFilePathAsync(item.Id, file.Name).ContinueWith(x => Uri = x.Result);
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
