using ClientApp.Models;
using ClientApp.Services;

namespace ClientApp.ViewModels
{
    public class ItemDetailViewModel : Base
    {
        ICloudService cloudService;

        public ItemDetailViewModel(TodoItem item = null)
        {
            if (item != null)
            {
                item = item;
            }
        }

        public TodoItem Item { get; set; }
    }
}
