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
		}

        #region Commands

        private ICommand logoutCommand;
        public ICommand LogoutCommand => logoutCommand ?? (logoutCommand = new Command(async () => await ExecuteLogoutCommand(), () => IsNotBusy && IsLoggedIn));

        #endregion

        #region Methods
     
        private async Task ExecuteLogoutCommand()
        {
            try
            {
                if (IsBusy)
                    return;

                IsBusy = true;

                Settings.AccessToken = null;
                Settings.AzureAuthToken = null;
                Settings.AzureUserId = null;
                Settings.RefreshToken = null;

                CoreMethods.SwitchOutRootNavigation(NavigationContainerNames.LoginContainer);
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
