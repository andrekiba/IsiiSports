using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using FreshMvvm;
using IsiiSports.DataObjects;
using IsiiSports.Interfaces;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Plugin.Connectivity;

namespace IsiiSports.Services.Stores
{
    public class BaseStore<T> : IBaseStore<T> where T : class, IBaseDataObject, new()
    {
        private IAzureService azureService;

        public virtual string Identifier => "Items";

        private IMobileServiceSyncTable<T> table;
        protected IMobileServiceSyncTable<T> Table => table ?? (table = AzureService.Client.GetSyncTable<T>());

        public void DropTable()
        {
            table = null;
        }

        #region IBaseStore implementation

        public async Task InitializeStore()
        {
            if (azureService == null)
                azureService = FreshIOC.Container.Resolve<IAzureService>();

            if (!azureService.IsInitialized)
                await azureService.InitializeAsync().ConfigureAwait(false);
        }

        public virtual async Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false)
        {
            await InitializeStore().ConfigureAwait(false);
            if (forceRefresh)
                await PullLatestAsync().ConfigureAwait(false);

            return await Table.ToEnumerableAsync().ConfigureAwait(false);
        }

        public virtual async Task<T> GetItemAsync(string id)
        {
            await InitializeStore().ConfigureAwait(false);
            await PullLatestAsync().ConfigureAwait(false);
            var items = await Table.Where(s => s.Id == id).ToListAsync().ConfigureAwait(false);

            if (items == null || items.Count == 0)
                return null;

            return items[0];
        }

        public virtual async Task<bool> InsertAsync(T item)
        {
            await InitializeStore().ConfigureAwait(false);
            await PullLatestAsync().ConfigureAwait(false);
            await Table.InsertAsync(item).ConfigureAwait(false);
            await SyncAsync().ConfigureAwait(false);
            return true;
        }

        public virtual async Task<bool> UpdateAsync(T item)
        {
            await InitializeStore().ConfigureAwait(false);
            await Table.UpdateAsync(item).ConfigureAwait(false);
            await SyncAsync().ConfigureAwait(false);
            return true;
        }

        public virtual async Task<bool> RemoveAsync(T item)
        {
            await InitializeStore().ConfigureAwait(false);
            await PullLatestAsync().ConfigureAwait(false);
            await Table.DeleteAsync(item).ConfigureAwait(false);
            await SyncAsync().ConfigureAwait(false);
            return true;
        }

        public async Task<bool> PullLatestAsync(CancellationToken token = default(CancellationToken))
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                Debug.WriteLine("Unable to pull items, we are offline");
                return false;
            }
            try
            {
                await Table.PullAsync($"all{Identifier}", Table.CreateQuery(), true, token, null).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to pull items, that is alright as we have offline capabilities: " + ex);
                return false;
            }
            return true;
        }

        public async Task<bool> SyncAsync(CancellationToken token = default(CancellationToken))
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                Debug.WriteLine("Unable to sync items, we are offline");
                return false;
            }
            try
            {
                await AzureService.Client.SyncContext.PushAsync(token).ConfigureAwait(false);
                if (!await PullLatestAsync(token).ConfigureAwait(false))
                    return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to sync items, that is alright as we have offline capabilities: " + ex);
                return false;
            }
            finally
            {
            }
            return true;
        }

        #endregion
    }
}
