using XamarinTodo.Services;
using XamarinTodo.iOS.Services;
using Microsoft.WindowsAzure.MobileServices.Files;
using Microsoft.WindowsAzure.MobileServices.Files.Metadata;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices.Files.Sync;
using Plugin.Media;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(iOSFileProvider))]
namespace XamarinTodo.iOS.Services
{
    public class iOSFileProvider : IFileProvider
    {
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
                // Xamarin Media Plugin to get a picture
                await CrossMedia.Current.Initialize();
                var file = await CrossMedia.Current.PickPhotoAsync();

                return file.Path;
            }
            catch (TaskCanceledException) { }
            return null;
        }

        public Task<string> GetItemFilesPathAsync()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TodoItemFiles");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return Task.FromResult(path);
        }
    }
}