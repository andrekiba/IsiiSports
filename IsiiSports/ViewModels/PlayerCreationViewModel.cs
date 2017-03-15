using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using IsiiSports.Base;
using IsiiSports.Helpers;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace IsiiSports.ViewModels
{
    public class PlayerCreationViewModel : BaseViewModel
    {
        public String UserName { get; set; }
        public String Email { get; set; }
        public String Nickname { get; set; }
        public String Description { get; set; }

        public PlayerCreationViewModel()
        {

        }

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);
            this.UserName = Settings.UserFullName ?? String.Empty;
            this.Email = Settings.PlayerEmail ?? String.Empty;
        }


        private ICommand submitPlayerCommand;
        public ICommand SubmitPlayerCommand => submitPlayerCommand ?? (submitPlayerCommand = new Command(ExecuteSubmitPlayerCommand));
        private void ExecuteSubmitPlayerCommand(object item)
        {
            App.Instance.CurrentPlayer.Description = this.Description;
            App.Instance.CurrentPlayer.Nickname = this.Nickname;
            Settings.SerializedPlayer = JsonConvert.SerializeObject(App.Instance.CurrentPlayer);
            MessagingCenter.Send(App.Instance, Messages.UserLoggedIn);
            CoreMethods.SwitchOutRootNavigation(NavigationContainerNames.MainContainer);
        }
    }
}
