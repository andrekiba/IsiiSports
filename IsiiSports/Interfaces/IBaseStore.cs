using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IsiiSports.Interfaces
{
    public interface IBaseStore<T>
    {
        Task InitializeStore();
        Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);
        Task<T> GetItemAsync(string id);
        Task<bool> InsertAsync(T item);
        Task<bool> UpdateAsync(T item);
        Task<bool> RemoveAsync(T item);
        Task<bool> SyncAsync(CancellationToken token = default(CancellationToken));
        void DropTable();
        string Identifier { get; }
    }
}
