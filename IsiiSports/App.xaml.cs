using Acr.UserDialogs;
using FreshMvvm;
using IsiiSports.Base;
using IsiiSports.Interfaces;
using IsiiSports.Services;
using IsiiSports.Services.Stores;
using IsiiSports.ViewModels;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using Xamarin.Forms;
using Device = Xamarin.Forms.Device;

namespace IsiiSports
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();
			SetupIoC();

			var loginPage = FreshPageModelResolver.ResolvePageModel<LoginViewModel>();
			var loginContainer = new FreshNavigationContainer(loginPage, NavigationContainerNames.LoginContainer);

			var tabbedNavigation = new FreshTabbedNavigationContainer();
			tabbedNavigation.AddTab<GamesViewModel>("Games", Device.OnPlatform("facebook", "facebook", "Assets/facebook"));
			tabbedNavigation.AddTab<TeamsViewModel>("Teams", Device.OnPlatform("google", "google", "Assets/google"));
			var mainContainer = tabbedNavigation;

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
