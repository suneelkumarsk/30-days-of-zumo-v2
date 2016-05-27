using Microsoft.WindowsAzure.MobileServices.Files;
using Microsoft.WindowsAzure.MobileServices.Files.Metadata;
using Microsoft.WindowsAzure.MobileServices.Files.Sync;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Plugin.Media;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using XamarinTodo.Services;

[assembly: Xamarin.Forms.Dependency(typeof(XamarinTodo.UWP.Services.UWPFileProvider))]
namespace XamarinTodo.UWP.Services
{
    public class UWPFileProvider : IFileProvider
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
                await CrossMedia.Current.Initialize();
                var file = await CrossMedia.Current.PickPhotoAsync();
                return file.Path;
            }
            catch (TaskCanceledException) { }
            return null;
        }

        public async Task<string> GetItemFilesPathAsync()
        {
            var folder = ApplicationData.Current.LocalFolder;
            var path = "TodoItemFiles";
            var result = await folder.TryGetItemAsync(path);
            if (result == null)
                result = await folder.CreateFolderAsync(path);
            return result.Name;
        }
    }
}
