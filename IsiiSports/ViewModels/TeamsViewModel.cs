using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using IsiiSports.DataObjects;
using MvvmHelpers;
using Xamarin.Forms;

namespace IsiiSports.ViewModels
{
    public class TeamsViewModel : BaseViewModel
    {
        #region Properties

        public ObservableRangeCollection<Team> Teams { get; set; } = new ObservableRangeCollection<Team>();

        #endregion

        #region Methods

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);

            if (!Teams.Any())
                this.LoadTeamsCommand.Execute(true);

        }

        #endregion

        #region Commands

        private ICommand itemTappedCommand;
        public ICommand ItemTappedCommand => itemTappedCommand ?? (itemTappedCommand = new Command(ExecuteItemTappedCommand));
        private void ExecuteItemTappedCommand(object item)
        {
            CoreMethods.PushPageModel<TeamViewModel>(item);
        }

        private ICommand loadTeamsCommand;
        public ICommand LoadTeamsCommand => loadTeamsCommand ?? (loadTeamsCommand = new Command<bool>(async forceRefresh => await ExecuteLoadTeamsCommand(forceRefresh)));
        private async Task ExecuteLoadTeamsCommand(bool forceRefresh)
        {
            if (IsBusy)
                return;
            try
            {
                LoadingMessage = "Loading Teams...";
                IsBusy = true;
                var teams = await AzureService.TeamStore.GetItemsAsync(forceRefresh);
                Teams.ReplaceRange(teams);
            }
            catch (Exception)
            {
                await CoreMethods.DisplayAlert("Sync Error", "Unable to sync Teams, you may be offline", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion
    }
}
