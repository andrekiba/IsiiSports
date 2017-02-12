using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IsiiSports.Helpers;
using IsiiSports.Models;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Plugin.Connectivity;

namespace IsiiSports.Services
{
    public class AzureService : IAzureService
    {
        private IMobileServiceSyncTable<Game> gameTable;
        private IMobileServiceSyncTable<Team> teamTable;
        private IMobileServiceSyncTable<Player> playerTable;
        private IMobileServiceSyncTable<GameField> gameFieldTable;

        #region Properties

        public MobileServiceClient Client { get; set; }
        public static string DbPath { get; set; } = "syncstore2.db";

        #endregion

        #region Methods

        private async Task Initialize()
        {
            if (Client?.SyncContext?.IsInitialized ?? false)
                return;

            const string appUrl = "https://mobile-da5ef11f-5046-4f48-835c-da199cd3c68f.azurewebsites.net/";

#if AUTH
            UseAuth = true;
            
            Client = new MobileServiceClient(appUrl, new AuthHandler());

            if (!string.IsNullOrWhiteSpace (Settings.AuthToken) && !string.IsNullOrWhiteSpace (Settings.UserId)) {
                Client.CurrentUser = new MobileServiceUser(Settings.UserId)
                {
                    MobileServiceAuthenticationToken = Settings.AuthToken
                };
            }
#else
            //Create our client
            Client = new MobileServiceClient(appUrl);
#endif

            //setup our local sqlite store and intialize our table
            SQLitePCL.Batteries.Init();
            var store = new MobileServiceSQLiteStore(DbPath);

            //Define table
            store.DefineTable<Game>();
            store.DefineTable<Team>();
            store.DefineTable<Player>();
            store.DefineTable<GameField>();

            //Initialize SyncContext
            await Client.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());

            //Get our sync table that will call out to azure
            gameTable = Client.GetSyncTable<Game>();
            teamTable = Client.GetSyncTable<Team>();
            playerTable = Client.GetSyncTable<Player>();
            gameFieldTable = Client.GetSyncTable<GameField>();
        }

       public async Task<IEnumerable<Game>> GetGames(bool sync = false)
        {
            await Initialize();

            if (sync && CrossConnectivity.Current.IsConnected)
            {
                await Client.SyncContext.PushAsync();
                //await poofTable.PullAsync("allGames" + Settings.UserId, poofTable.CreateQuery());
                await gameTable.PullAsync("allGames", gameTable.CreateQuery());
            }

            return await gameTable.ToEnumerableAsync();
                        
        }

        #endregion
    }
}
