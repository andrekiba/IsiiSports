using System.Threading.Tasks;
using System.Windows.Input;
using IsiiSports.Base;
using IsiiSports.Interfaces;
using Xamarin.Forms;

namespace IsiiSports.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        #region Properties

        public string UserName { get; set; }
        public string Password { get; set; }

        #endregion

        #region Commands

        private ICommand loginCommand;
        public ICommand LoginCommand => loginCommand ?? (loginCommand = new Command<string>(async authProvider => await ExecuteLoginCommand(authProvider), authProvider => !IsBusy));

        private ICommand skipLoginCommand;

        public ICommand SkipLoginCommand => skipLoginCommand ?? (skipLoginCommand = new Command(ExecuteSkipLoginCommand));
        private void ExecuteSkipLoginCommand()
        {
            CoreMethods.SwitchOutRootNavigation(NavigationContainerNames.MainContainer);
        }

        #endregion

        #region Methods

        private async Task ExecuteLoginCommand(string authProvider)
        {
            var loggedIn = await AzureService.LoginAsync(authProvider);
            if (loggedIn)
            {
                //SetUserSettings(result);

                CoreMethods.SwitchOutRootNavigation(NavigationContainerNames.MainContainer);
            }
            else
            {
                await CoreMethods.DisplayAlert("Errore durante il login...", "Error", "OK");
            }
        }

        //private void SetUserSettings(UserLoggedDTO dto)
        //{
        //	Settings.IdUser = dto.IdUser;
        //	Settings.UserName = UserName;
        //	Settings.Password = Password;
        //	Settings.UserFullName = dto.FirstName + " " + dto.LastName;
        //}

        #endregion
    }
}
