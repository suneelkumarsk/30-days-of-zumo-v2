using Microsoft.WindowsAzure.MobileServices.Files;
using Microsoft.WindowsAzure.MobileServices.Files.Metadata;
using Microsoft.WindowsAzure.MobileServices.Files.Sync;
using System.Threading.Tasks;
using Xamarin.Forms;
using XamarinTodo.Helpers;

namespace XamarinTodo.Services
{
    class TodoItemFileSyncHandler : IFileSyncHandler
    {
        private IFileProvider fileProvider;
        private ICloudService cloudService;

        public TodoItemFileSyncHandler()
        {
            fileProvider = DependencyService.Get<IFileProvider>();
            cloudService = ServiceLocator.Instance.Resolve<ICloudService>();
        }

        public Task<IMobileServiceFileDataSource> GetDataSource(MobileServiceFileMetadata metadata)
            => fileProvider.GetFileDataSource(metadata);

        public async Task ProcessFileSynchronizationAction(MobileServiceFile file, FileSynchronizationAction action)
        {
            if (action == FileSynchronizationAction.Delete)
                await fileProvider.DeleteLocalFileAsync(file);
            else
                await cloudService.DownloadItemFileAsync(file);
        }
    }
}
