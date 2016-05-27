using Microsoft.WindowsAzure.MobileServices.Files;
using PCLStorage;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamarinTodo.Services
{
    public class FileHelper
    {
        public static async Task<string> CopyItemFileAsync(string itemId, string filePath)
        {
            IFolder localStorage = FileSystem.Current.LocalStorage;

            string fileName = Path.GetFileName(filePath);
            string targetPath = await GetLocalFilePathAsync(itemId, fileName);

            var sourceFile = await FileSystem.Current.GetFileFromPathAsync(filePath);
            var sourceStream = await sourceFile.OpenAsync(FileAccess.Read);

            var targetFile = await localStorage.CreateFileAsync(targetPath, CreationCollisionOption.ReplaceExisting);

            using (var targetStream = await targetFile.OpenAsync(FileAccess.ReadAndWrite))
            {
                await sourceStream.CopyToAsync(targetStream);
            }

            return targetPath;
        }

        public static async Task<string> GetLocalFilePathAsync(string itemId, string fileName)
        {
            IFileProvider fileProvider = DependencyService.Get<IFileProvider>();

            string recordFilesPath = Path.Combine(await fileProvider.GetItemFilesPathAsync(), itemId);

            var checkExists = await FileSystem.Current.LocalStorage.CheckExistsAsync(recordFilesPath);
            if (checkExists == ExistenceCheckResult.NotFound)
            {
                await FileSystem.Current.LocalStorage.CreateFolderAsync(recordFilesPath, CreationCollisionOption.ReplaceExisting);
            }

            return Path.Combine(recordFilesPath, fileName);
        }

        public static async Task DeleteLocalFileAsync(MobileServiceFile fileName)
        {
            string localPath = await GetLocalFilePathAsync(fileName.ParentId, fileName.Name);
            var checkExists = await FileSystem.Current.LocalStorage.CheckExistsAsync(localPath);

            if (checkExists == ExistenceCheckResult.FileExists)
            {
                var file = await FileSystem.Current.LocalStorage.GetFileAsync(localPath);
                await file.DeleteAsync();
            }
        }
    }
}
