using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FreshMvvm;
using IsiiSports.Services;
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

            //MainPage = new IsiiSports.MainPage();

            FreshIOC.Container.Register<IAzureService, AzureService>();

            //var masterDetailGames = new FreshMasterDetailNavigationContainer();
            //masterDetailGames.Init("Games");
            //masterDetailGames.AddPage<GamesViewModel>("Games", null);
            //masterDetailGames.AddPage<GameViewModel>("Game", null);

            //var masterDetailTeams = new FreshMasterDetailNavigationContainer();
            //masterDetailTeams.Init("Teams");
            //masterDetailTeams.AddPage<TeamsViewModel>("Teams", null);
            //masterDetailTeams.AddPage<TeamViewModel>("Team", null);


            var tabbedNavigation = new FreshTabbedNavigationContainer();
            //tabbedNavigation.AddTab<GamesViewModel>("Games", Device.OnPlatform("poofTab.png", "poofTab.png", "Assets/poofTab.png") );
            //tabbedNavigation.AddTab<TeamsViewModel>("Teams", Device.OnPlatform("poofListTab.png", "poofListTab.png", "Assets/poofListTab.png"));

            tabbedNavigation.AddTab<GamesViewModel>("Games", Device.OnPlatform("", "", ""));
            tabbedNavigation.AddTab<TeamsViewModel>("Teams", Device.OnPlatform("", "", ""));

            //tabbedNavigation.BarBackgroundColor = (Color)Resources[@"BarTint"];
            //tabbedNavigation.BarTextColor = (Color)Resources[@"Back"];
            //tabbedNavigation.Title = "POOF";
            MainPage = tabbedNavigation;

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
