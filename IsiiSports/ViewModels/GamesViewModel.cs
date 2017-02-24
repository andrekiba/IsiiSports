using IsiiSports.DataObjects;
using MvvmHelpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace IsiiSports.ViewModels
{
    public class GamesViewModel : BaseViewModel
    {
        #region Properties
        public ObservableRangeCollection<Game> Games { get; set; } = new ObservableRangeCollection<Game>();
        
        #endregion

        #region Methods
        
        public override void Init(object initData)
        {
            base.Init(initData);
        }

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);

            if (!Games.Any())
                LoadGamesCommand.Execute(null);
        }

        #endregion

        #region Commands

        private ICommand itemTappedCommand;
        public ICommand ItemTappedCommand => itemTappedCommand ?? (itemTappedCommand = new Command(ExecuteItemTappedCommand));
        private void ExecuteItemTappedCommand(object item)
        {
            CoreMethods.PushPageModel<GameViewModel>(item);
        }

        private ICommand loadGamesCommand;
        public ICommand LoadGamesCommand => loadGamesCommand ?? (loadGamesCommand = new Command<bool>(async forceRefresh => await ExecuteLoadGamesCommand(forceRefresh)));
        private async Task ExecuteLoadGamesCommand(bool forceRefresh)
        {
            if (IsBusy)
                return;
            try
            {
                LoadingMessage = "Loading Games...";
                IsBusy = true;
                var games = await AzureService.GameStore.GetItemsAsync(forceRefresh);
                Games.ReplaceRange(games);
            }
            catch (Exception ex)
            {
                await CoreMethods.DisplayAlert("Sync Error", "Unable to sync Games, you may be offline", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion
    }
}
