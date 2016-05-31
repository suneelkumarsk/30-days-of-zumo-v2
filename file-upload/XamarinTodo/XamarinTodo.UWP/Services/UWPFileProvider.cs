using Microsoft.WindowsAzure.MobileServices.Files;
using Microsoft.WindowsAzure.MobileServices.Files.Metadata;
using Microsoft.WindowsAzure.MobileServices.Files.Sync;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Plugin.Media;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using XamarinTodo.Services;

[assembly: Xamarin.Forms.Dependency(typeof(XamarinTodo.UWP.Services.UWPFileProvider))]
namespace XamarinTodo.UWP.Services
{
    public class UWPFileProvider : IFileProvider
    {
        /// <summary>
        /// Copy the newly found file into our temporary area
        /// </summary>
        /// <param name="itemId">The ID of the item that this file will be associated with</param>
        /// <param name="filePath">The path to the original file</param>
        /// <returns>The path to the copied file</returns>
		public async Task<string> CopyItemFileAsync(string itemId, string filePath)
		{
			string fileName = Path.GetFileName(filePath);
			string targetPath = await GetLocalFilePathAsync(itemId, fileName);

            var sourceFolder = await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(filePath));
            var sourceFile = await sourceFolder.GetFileAsync(Path.GetFileName(filePath));
            var sourceStream = await sourceFile.OpenStreamForReadAsync();

            var targetFolder = await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(targetPath));
            var targetFile = await targetFolder.CreateFileAsync(Path.GetFileName(filePath), CreationCollisionOption.ReplaceExisting);
			using (var targetStream = await targetFile.OpenStreamForWriteAsync())
			{
				await sourceStream.CopyToAsync(targetStream);
			}

			return targetPath;
		}

        /// <summary>
        /// Delete an existing mobile apps associated file
        /// </summary>
        /// <param name="file">The file to delete</param>
        /// <returns>Task (Async)</returns>
		public async Task DeleteLocalFileAsync(MobileServiceFile file)
		{
			string localPath = await GetLocalFilePathAsync(file.ParentId, file.Name);
            var storageFolder = await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(localPath));
            try
            {
                var storageFile = await storageFolder.GetFileAsync(Path.GetFileName(localPath));
                await storageFile.DeleteAsync();
            }
            catch (FileNotFoundException) { }
            // UnauthorizedAccessException is still thrown, but should never happen
		}

        /// <summary>
        /// Download a file from blob storage and store it in local storage
        /// </summary>
        /// <typeparam name="T">The type of the table controller</typeparam>
        /// <param name="table">The sync table reference</param>
        /// <param name="file">The file to download</param>
        /// <param name="filename">The local storage location of the file</param>
        /// <returns></returns>
        public async Task DownloadFileAsync<T>(IMobileServiceSyncTable<T> table, MobileServiceFile file, string filename)
        {
            var path = await GetLocalFilePathAsync(file.ParentId, file.Name);
            await table.DownloadFileAsync(file, path);
        }

        public async Task<IMobileServiceFileDataSource> GetFileDataSource(MobileServiceFileMetadata metadata)
        {
            var path = await GetLocalFilePathAsync(metadata.ParentDataItemId, metadata.FileName);
            return new PathMobileServiceFileDataSource(path);
        }

        /// <summary>
        /// Ask the user for an image location
        /// </summary>
        /// <returns>The path to the image (or null if cancelled)</returns>
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

        /// <summary>
        /// Get the path to the local storage for files (create folder if it doesn't exist
        /// </summary>
        /// <returns>The path to the local storage area</returns>
        public async Task<string> GetItemFilesPathAsync()
        {
            var folder = ApplicationData.Current.LocalFolder;
            var path = "TodoItemFiles";
            var result = await folder.TryGetItemAsync(path);
            if (result == null)
                result = await folder.CreateFolderAsync(path);
            return result.Path;
        }

        /// <summary>
        /// Get the local storage path for a specific file attached to a specific item, creating the folder if necessary
        /// </summary>
        /// <param name="itemId">The ID of the item the file is attached to</param>
        /// <param name="fileName">The name of the file</param>
        /// <returns></returns>
		public async Task<string> GetLocalFilePathAsync(string itemId, string fileName)
		{
			string recordFilesPath = Path.Combine(await GetItemFilesPathAsync(), itemId);

            try
            {
                var storagePath = await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(recordFilesPath));
                await storagePath.CreateFolderAsync(Path.GetFileName(recordFilesPath), CreationCollisionOption.FailIfExists);
            }
            catch (Exception) { /* if CreateFolderAsync exists, then don't fail */ }

			return Path.Combine(recordFilesPath, fileName);
		}
    }
}
