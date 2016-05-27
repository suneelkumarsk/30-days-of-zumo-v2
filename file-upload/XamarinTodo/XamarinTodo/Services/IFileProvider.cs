using Microsoft.WindowsAzure.MobileServices.Files;
using Microsoft.WindowsAzure.MobileServices.Files.Metadata;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System.Threading.Tasks;

namespace XamarinTodo.Services
{
    public interface IFileProvider
    {
        Task<string> GetItemFilesPathAsync();
        Task<IMobileServiceFileDataSource> GetFileDataSource(MobileServiceFileMetadata metadata);
        Task<string> GetImageAsync();
        Task DownloadFileAsync<T>(IMobileServiceSyncTable<T> table, MobileServiceFile file, string filename);
    }
}
