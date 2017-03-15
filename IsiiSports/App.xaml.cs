using System.Linq;
using Acr.UserDialogs;
using FreshMvvm;
using IsiiSports.Base;
using IsiiSports.Controls;
using IsiiSports.DataObjects;
using IsiiSports.Helpers;
using IsiiSports.Interfaces;
using IsiiSports.Services;
using IsiiSports.Services.Stores;
using IsiiSports.ViewModels;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using Newtonsoft.Json;
using Xamarin.Forms;
using Device = Xamarin.Forms.Device;

namespace IsiiSports
{
    public partial class App : Application
    {
        public static App Instance { get; private set; }
        public Player CurrentPlayer { get; set; }

        public App()
        {
            InitializeComponent();

            Instance = this;

            SetupIoC();

            var loginPage = FreshPageModelResolver.ResolvePageModel<LoginViewModel>();
            var playerCreationPage = FreshPageModelResolver.ResolvePageModel<PlayerCreationViewModel>();
            var loginContainer = new FreshNavigationContainer(loginPage, NavigationContainerNames.LoginContainer);
            var playerCreationContainer = new FreshNavigationContainer(playerCreationPage, NavigationContainerNames.PlayerCreationContainer);

            //var mainContainer = new FreshTabbedFONavigationContainer("ISII Sports", NavigationContainerNames.MainContainer);
            //var mainContainer = new FreshTabbedNavigationContainer(NavigationContainerNames.MainContainer);
            var mainContainer = new BottomBarTabbedFoNavigationContainer("ISII Sports", NavigationContainerNames.MainContainer);
            mainContainer.AddTab<GamesViewModel>("Games", Device.OnPlatform("games.png", "games.png", ""));
            mainContainer.AddTab<TeamsViewModel>("Teams", Device.OnPlatform("teams.png", "teams.png", ""));
            mainContainer.AddTab<InfoViewModel>("Info", Device.OnPlatform("info.png", "info.png", ""));

            var tabs = mainContainer.TabbedPages.ToList();
            tabs[0].SetTabColor(null);
            tabs[1].SetTabColor(Color.FromHex("#7B1FA2"));
            tabs[2].SetTabColor(Color.FromHex("#FF5252"));

            if (Settings.IsLoggedIn)
            {
                //TODO: Controllare questa logica attentamente
                var serializedPlayer = Settings.SerializedPlayer;
                if (string.IsNullOrEmpty(serializedPlayer))
                    MainPage = playerCreationContainer;
                else
                {
                    var player = JsonConvert.DeserializeObject<Player>(serializedPlayer);
                    if (player.Email == Settings.PlayerEmail)
                    {
                        App.Instance.CurrentPlayer = player;
                        MessagingCenter.Send(App.Instance, Messages.UserLoggedIn);
                        MainPage = mainContainer;
                    }
                    else
                    {
                        MainPage = loginContainer;
                    }
                }
            }
            else
                MainPage = loginContainer;
        }

        private static void SetupIoC()
        {
            FreshIOC.Container.Register<IAzureService, AzureService>();
            FreshIOC.Container.Register<IPlayerStore, PlayerStore>();
            FreshIOC.Container.Register<ITeamStore, TeamStore>();
            FreshIOC.Container.Register<IGameStore, GameStore>();
            FreshIOC.Container.Register<IGameFieldStore, GameFieldStore>();
            FreshIOC.Container.Register<IGameResultStore, GameResultStore>();
            FreshIOC.Container.Register(UserDialogs.Instance);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            MobileCenter.Start(typeof(Analytics), typeof(Crashes));
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
