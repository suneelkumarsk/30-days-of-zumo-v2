using Android.Content;
using Microsoft.WindowsAzure.MobileServices.Files;
using Microsoft.WindowsAzure.MobileServices.Files.Metadata;
using Microsoft.WindowsAzure.MobileServices.Files.Sync;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Plugin.Media;
using System;
using System.IO;
using System.Threading.Tasks;
using XamarinTodo.Droid.Services;
using XamarinTodo.Services;

[assembly: Xamarin.Forms.Dependency(typeof(DroidFileProvider))]
namespace XamarinTodo.Droid.Services
{
    public class DroidFileProvider : IFileProvider
    {
        // Storage for the UI Context
        public Context UIContext { get; set; }

        public async Task DownloadFileAsync<T>(IMobileServiceSyncTable<T> table, MobileServiceFile file, string filename)
        {
            var path = await FileHelper.GetLocalFilePathAsync(file.ParentId, file.Name);
            await table.DownloadFileAsync(file, path);
        }

        public async Task<IMobileServiceFileDataSource> GetFileDataSource(MobileServiceFileMetadata metadata)
        {
            var path = await FileHelper.GetLocalFilePathAsync(metadata.ParentDataItemId, metadata.FileName);
            return new PathMobileServiceFileDataSource(path);
        }

        public async Task<string> GetImageAsync()
        {
            try
            {
                if (UIContext != null)
                {
                    // Xamarin Media Plugin to get a picture
                    await CrossMedia.Current.Initialize();
                    var file = await CrossMedia.Current.PickPhotoAsync();

                    return file.Path;
                }
            }
            catch (TaskCanceledException) { }
            return null;
        }

        public Task<string> GetItemFilesPathAsync()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var filesPath = Path.Combine(appData, "TodoItemFiles");
            if (!Directory.Exists(filesPath))
                Directory.CreateDirectory(filesPath);
            return Task.FromResult(filesPath);
        }
    }
}