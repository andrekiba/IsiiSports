using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreshMvvm;
using IsiiSports.Auth;
using IsiiSports.Base;
using IsiiSports.DataObjects;
using IsiiSports.Helpers;
using IsiiSports.Interfaces;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Xamarin.Forms;

namespace IsiiSports.Services
{
    public class AzureService : IAzureService
    {
        #region Fields

        private IPlayerStore playerStore;
        private ITeamStore teamStore;
        private IGameStore gameStore;
        private IGameFieldStore gameFieldStore;
        private IGameResultStore gameResultStore;
        private readonly object locker = new object();

        #endregion

        #region Properties

        public static MobileServiceClient Client { get; protected set; }
        public AuthUser AuthUser { get; protected set; }
        public bool IsInitialized { get; private set; }
        public MobileServiceAuthenticationProvider AuthProvider { get; protected set; }
        public static bool UseAuth { get; protected set; } = true;
        public static string DbPath { get; set; } = "syncstore2.db";
      
        public IPlayerStore PlayerStore => playerStore ?? (playerStore = FreshIOC.Container.Resolve<IPlayerStore>());       
        public ITeamStore TeamStore => teamStore ?? (teamStore = FreshIOC.Container.Resolve<ITeamStore>());
        public IGameStore GameStore => gameStore ?? (gameStore = FreshIOC.Container.Resolve<IGameStore>());
        public IGameFieldStore GameFieldStore => gameFieldStore ?? (gameFieldStore = FreshIOC.Container.Resolve<IGameFieldStore>());
        public IGameResultStore GameResultStore => gameResultStore ?? (gameResultStore = FreshIOC.Container.Resolve<IGameResultStore>());

        #endregion

        #region Methods

        public async Task InitializeAsync()
        {
            MobileServiceSQLiteStore store;
            lock (locker)
            {
                if (IsInitialized)
                    return;
#if AUTH
            UseAuth = true;
            
            Client = new MobileServiceClient(Keys.AppUrl, new AuthHandler());

            if (!string.IsNullOrWhiteSpace (Settings.AzureAuthToken) && !string.IsNullOrWhiteSpace (Settings.AzureUserId)) {
                Client.CurrentUser = new MobileServiceUser(Settings.AzureUserId)
                {
                    MobileServiceAuthenticationToken = Settings.AzureAuthToken
                };
            }
#else
                //Create our client
                Client = new MobileServiceClient(Keys.AppUrl);
#endif

                //setup our local sqlite store and intialize our table
                SQLitePCL.Batteries.Init();
                store = new MobileServiceSQLiteStore(DbPath);

                //Define tables
                store.DefineTable<Game>();
                store.DefineTable<Team>();
                store.DefineTable<Player>();
                store.DefineTable<GameField>();
                store.DefineTable<GameResult>();

                IsInitialized = true;
            }

            //InitializeAsync SyncContext
            await Client.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler()).ConfigureAwait(false);
        }

        public async Task<bool> SyncAllAsync()
        {
            if (!IsInitialized)
                await InitializeAsync();

            var taskList = new List<Task<bool>>
            {
                PlayerStore.SyncAsync(),
                TeamStore.SyncAsync(),
                GameStore.SyncAsync(),
                GameFieldStore.SyncAsync(),
                GameResultStore.SyncAsync()
            };

            var successes = await Task.WhenAll(taskList).ConfigureAwait(false);
            return successes.Any(x => !x);
        }

        public Task DropEverythingAsync()
        {
            PlayerStore.DropTable();
            TeamStore.DropTable();
            GameStore.DropTable();
            GameFieldStore.DropTable();
            GameResultStore.DropTable();

            IsInitialized = false;
            return Task.FromResult(true);
        }

        #region Login
        public async Task<bool> LoginAsync(string authProvider = null)
        {
            await InitializeAsync();

            //se ho già tutte le informazioni necessarie creo direttamente l'utente
            if (!string.IsNullOrEmpty(Settings.AzureUserId) && !string.IsNullOrEmpty(Settings.AzureAuthToken))
            {
                Client.CurrentUser = new MobileServiceUser(Settings.AzureUserId)
                {
                    MobileServiceAuthenticationToken = Settings.AzureAuthToken
                };
            }

            var auth = DependencyService.Get<IAuthentication>();

			if (!string.IsNullOrEmpty(authProvider)) 
				Settings.AuthProvider = authProvider;			

			var authUser = await auth.LoginAsync(Client, Settings.AuthProvider);

            if (authUser != null)
            {
                AuthUser = authUser;

                Settings.AzureAuthToken = authUser.MobileServiceUser.MobileServiceAuthenticationToken;
                Settings.AzureUserId = authUser.MobileServiceUser.UserId;
                Settings.AccessToken = Settings.AuthProvider == MobileServiceAuthenticationProvider.Google.ToString() ? authUser.GoogleUser.AccessToken : authUser.FacebookUser.AccessToken;
                //Settings.RefreshToken = Settings.AuthProvider == "Google" ?
                Settings.UserId = Settings.AuthProvider == MobileServiceAuthenticationProvider.Google.ToString() ? authUser.GoogleUser.UserId : authUser.FacebookUser.UserId;

                return true;
            }

            AuthUser = null;
            Settings.AuthProvider = null;
            Settings.AzureAuthToken = string.Empty;
            Settings.AzureUserId = string.Empty;
            Settings.AccessToken = string.Empty;
            Settings.UserId = string.Empty;

            return false;
        }

        #endregion

        #endregion
    }
}
