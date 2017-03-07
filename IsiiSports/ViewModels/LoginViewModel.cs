using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using IsiiSports.Auth;
using IsiiSports.Base;
using IsiiSports.Helpers;
using Xamarin.Forms;

namespace IsiiSports.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        #region Properties

        #endregion

        #region Commands

        private ICommand loginCommand;
        public ICommand LoginCommand => loginCommand ?? (loginCommand = new Command<string>(async authProvider => await ExecuteLoginCommand(authProvider), authProvider => IsNotBusy));

        private ICommand skipLoginCommand;
        public ICommand SkipLoginCommand => skipLoginCommand ?? (skipLoginCommand = new Command(ExecuteSkipLoginCommand, () => IsNotBusy));

        #endregion

        #region Methods
        private void ExecuteSkipLoginCommand()
        {
            CoreMethods.SwitchOutRootNavigation(NavigationContainerNames.MainContainer);
        }

        private async Task ExecuteLoginCommand(string authProvider)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            var loggedIn = await AzureService.LoginAsync(authProvider);

            if (loggedIn)
                CoreMethods.SwitchOutRootNavigation(NavigationContainerNames.MainContainer);
            else
                await UserDialogs.Instance.AlertAsync("Errore durante il login...", "Error");

            IsBusy = false;
        }

        #endregion
    }
}
