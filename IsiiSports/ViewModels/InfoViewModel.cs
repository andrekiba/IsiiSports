using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using IsiiSports.Base;
using IsiiSports.Helpers;
using Xamarin.Forms;

namespace IsiiSports.ViewModels
{
    public class InfoViewModel : BaseViewModel
    {
        #region Properties

        public bool IsLoggedIn => Settings.IsLoggedIn;
        public string Name => App.Instance.CurrentPlayer != null ? App.Instance.CurrentPlayer.Name : "Not logged in!";
        public string Nickname => App.Instance.CurrentPlayer != null ? App.Instance.CurrentPlayer.Nickname : null;
        public string Email => App.Instance.CurrentPlayer != null ? App.Instance.CurrentPlayer.Email : null;

        #endregion

        public InfoViewModel()
        {
			MessagingCenter.Subscribe<App>(this, Messages.UserLoggedIn, (app) =>
			{
				if (App.Instance.CurrentPlayer != null)
				{
					RaisePropertyChanged(null);
				}

				Device.BeginInvokeOnMainThread(() =>
				{
					MessagingCenter.Unsubscribe<App>(this, Messages.UserLoggedIn);
				});
			});
        }

        #region Commands

        private ICommand logoutCommand;
		public ICommand LogoutCommand => logoutCommand ?? 
			(logoutCommand = new DependentCommand(
				async () => await ExecuteLogoutCommand(),
				() => IsNotBusy && IsLoggedIn,
				this,
				() => IsNotBusy, () => IsLoggedIn)
		    );


        #endregion

        #region Methods

        private async Task ExecuteLogoutCommand()
        {
            try
            {
                if (IsBusy)
                    return;

                IsBusy = true;

                var confirm = await UserDialogs.Instance.ConfirmAsync("Sei sicuro di volere uscire?", "Attenzione!");

                if (confirm)
                {
                    var loggedOut = await AzureService.LogoutAsync();

                    if(loggedOut)
                        CoreMethods.SwitchOutRootNavigation(NavigationContainerNames.LoginContainer);
                    else
                        await UserDialogs.Instance.AlertAsync("Errore durante il logout...", "Error", "OK");
                }
            }
            catch (Exception)
            {
                await UserDialogs.Instance.AlertAsync("Errore durante il logout...", "Error", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion
    }
}
