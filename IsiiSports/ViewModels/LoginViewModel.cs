using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using IsiiSports.Auth;
using IsiiSports.Base;
using IsiiSports.DataObjects;
using IsiiSports.Helpers;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace IsiiSports.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
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
            {
                //Controllo se ho già le info del player
                var serializedPlayer = Settings.SerializedPlayer;
                if (!string.IsNullOrEmpty(serializedPlayer))
                {
                    var player = JsonConvert.DeserializeObject<Player>(serializedPlayer);
                    if (player.Email == Settings.PlayerEmail) //TODO: ci sono casi in cui può capitare che questa condizione non sia soddisfatta?
                    {
                        //I dati che ho nei settings sono corretti, accedo all'app
                        App.Instance.CurrentPlayer = player;
                        MessagingCenter.Send(App.Instance, Messages.UserLoggedIn);
                        CoreMethods.SwitchOutRootNavigation(NavigationContainerNames.MainContainer);
                    }
                    else
                    {
                        NavigateToPlayerCreation();
                    }
                }
                else
                {
                    NavigateToPlayerCreation();
                }
            }
            else
                await UserDialogs.Instance.AlertAsync("Errore durante il login...", "Error");

			IsBusy = false;
		}

        /// <summary>
        /// Naviga verso la pagina di creazione del player
        /// </summary>
        private void NavigateToPlayerCreation()
        {
            CoreMethods.SwitchOutRootNavigation(NavigationContainerNames.PlayerCreationContainer);
        }

        #endregion
    }
}
