using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using IsiiSports.Base;
using IsiiSports.DataObjects;
using IsiiSports.Helpers;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace IsiiSports.ViewModels
{
    public class PlayerCreationViewModel : BaseViewModel
    {
        #region Properties

        public string UserName { get; set; }
        public string Email { get; set; }
        public string Nickname { get; set; }
        public string Description { get; set; }
		public string ProfileImageUrl { get; set;}
        
        #endregion

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);
            UserName = Settings.UserFullName ?? string.Empty;
            Email = Settings.PlayerEmail ?? string.Empty;
			ProfileImageUrl = Settings.ProfileImageUrl ?? null;
        }

        private ICommand submitPlayerCommand;
        public ICommand SubmitPlayerCommand => submitPlayerCommand ?? (submitPlayerCommand = new DependentCommand(
            async () => await ExecuteSubmitPlayerCommand(),
            () => IsNotBusy && !string.IsNullOrEmpty(Nickname),
            this,
            () => Nickname
        ));

        private async Task ExecuteSubmitPlayerCommand()
        {
            try
            {
                if (IsBusy)
                    return;

                IsBusy = true;

                using (UserDialogs.Instance.Loading("Creazione del player in corso..."))
                {
					#region Azure

					//vedo in base alla mail se devo creare un nuovo player oppure no
					var player = await AzureService.PlayerStore.GetPlayerByMail(Settings.PlayerEmail);
                    if (player != null)
                    {
                        //setto il player corrente dell'app
                        App.Instance.CurrentPlayer = player;
                        Settings.PlayerId = player.Id;
                    }
                    else
                    {
                        var newPlayer = new Player
                        {
                            Name = Settings.UserFullName,
                            Nickname = Nickname,
                            Description = Description,
                            Email = Settings.PlayerEmail,
                            ProfileImageUrl = Settings.ProfileImageUrl
                        };

                        //salvo il player su Azure
                        await AzureService.PlayerStore.InsertAsync(newPlayer);
                        //setto il player corrente dell'app
                        App.Instance.CurrentPlayer = newPlayer;
                        Settings.PlayerId = newPlayer.Id;
                    }

					#endregion

					//var newPlayer = new Player
					//{
					//	Name = Settings.UserFullName,
					//	Nickname = Nickname,
					//	Description = Description,
					//	Email = Settings.PlayerEmail,
					//	ProfileImageUrl = Settings.ProfileImageUrl
					//};

					////setto il player corrente dell'app
					//App.Instance.CurrentPlayer = newPlayer;
					//Settings.PlayerId = newPlayer.Id;

                    //serializzo il player creato per utilizzarlo in futuro
                    Settings.SerializedPlayer = JsonConvert.SerializeObject(App.Instance.CurrentPlayer);
                }              

                MessagingCenter.Send(App.Instance, Messages.UserLoggedIn);
                CoreMethods.SwitchOutRootNavigation(NavigationContainerNames.MainContainer);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await UserDialogs.Instance.AlertAsync("Errore durante la creazione del player...", "Error");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
