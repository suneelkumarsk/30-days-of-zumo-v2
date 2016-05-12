using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.XForms.Models;

namespace Client.XForms.Services
{
    public class Mockservice : ICloudService
    {
        private List<Todoitem> items { get; set; } = new List<Todoitem>();

        public Mockservice()
        {
            if (items.Count == 0)
            {
                // Initialize the array with something...
            }
        }

        #region ICloudService
        public Task<Todoitem> AddOrUpdateItemAsync(Todoitem item)
        {
            if (item.Id == null)
            {
                item.Id = Guid.NewGuid().ToString("N");
                item.CreatedAt = DateTimeOffset.Now;
                item.UpdatedAt = DateTimeOffset.Now;
                item.AzureVersion = item.UpdatedAt.GetHashCode().ToString();
                items.Add(item);
            }
            else
            {
                item.UpdatedAt = DateTimeOffset.Now;
                item.AzureVersion = item.UpdatedAt.GetHashCode().ToString();
                var oldEntry = items.FirstOrDefault(x => x.Id == item.Id);
                items.Remove(oldEntry);
                items.Add(item);
            }
            return Task.FromResult(item);
        }

        public Task DeleteItemAsync(Todoitem item)
        {
            var oldEntry = items.FirstOrDefault(x => x.Id == item.Id);
            items.Remove(oldEntry);
            return null;
        }

        public Task<IEnumerable<Todoitem>> GetAllItemsAsync()
        {
            IEnumerable<Todoitem> entries = items.AsEnumerable();
            return Task.FromResult(entries);
        }

        public Task LoginAsync()
        {
            return null;
        }

        public Task LogoutAsync()
        {
            return null;
        }

        public Task SynchronizeItemsAsync()
        {
            return null;
        }
        #endregion
    }
}
