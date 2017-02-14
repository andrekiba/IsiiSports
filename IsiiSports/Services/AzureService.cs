using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IsiiSports.Auth;
using IsiiSports.Helpers;
using IsiiSports.Models;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Plugin.Connectivity;
using Xamarin.Forms;

namespace IsiiSports.Services
{
    public class AzureService : IAzureService
    {
        private IMobileServiceSyncTable<Game> gameTable;
        private IMobileServiceSyncTable<Team> teamTable;
        private IMobileServiceSyncTable<Player> playerTable;
        private IMobileServiceSyncTable<GameField> gameFieldTable;

        #region Properties

        public MobileServiceClient Client { get; protected set; }

        public MobileServiceAuthenticationProvider AuthProvider { get; protected set; }
        public static bool UseAuth { get; protected set; } = true;
        public static string DbPath { get; protected set; } = "syncstore2.db";

        #endregion

        #region Methods

        private async Task Initialize()
        {
            if (Client?.SyncContext?.IsInitialized ?? false)
                return;

            const string appUrl = "https://isiisports.azurewebsites.net/";

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

            //Define tables
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

        public async Task<bool> LoginAsync(string authProvider)
        {
            await Initialize();

            var auth = DependencyService.Get<IAuthentication>();

            AuthProvider = (MobileServiceAuthenticationProvider) Enum.Parse(typeof(MobileServiceAuthenticationProvider), authProvider);

            var user = await auth.LoginAsync(Client, AuthProvider);

            if (user == null)
            {
                Settings.AuthToken = string.Empty;
                Settings.UserId = string.Empty;
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("Login Error", "Unable to login, please try again", "OK");
                });
                return false;
            }

            //var socialLoginResult = await GetUserData();

            Settings.AuthToken = user.MobileServiceAuthenticationToken;
            Settings.UserId = user.UserId;

            return true;
        }

        public Task<bool> LoginAsync()
        {
            return LoginAsync(AuthProvider.ToString());
        }

        #endregion
    }
}
