using IsiiSports.DataObjects;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace IsiiSports.ViewModels
{
    public class GamesViewModel : BaseViewModel
    {
        #region Properties
        public ObservableRangeCollection<Game> GameList { get; set; } = new ObservableRangeCollection<Game>();
        #endregion

        #region Methods
        public GamesViewModel()
        {

        }

        public override async void Init(object initData)
        {
            if (this.GameList == null)
                return;

            //TODO: Sto facendo await dentro ad un metodo che ritorna void
            var result = await AzureService.GameStore.GetItemsAsync();
            this.GameList.ReplaceRange(result);
        }
        #endregion

        #region Commands

        private ICommand itemTappedCommand;
        public ICommand ItemTappedCommand => itemTappedCommand ?? (itemTappedCommand = new Command(ExecuteItemTappedCommand));
        private void ExecuteItemTappedCommand(object item)
        {
            base.CoreMethods.PushPageModel<GameViewModel>(item, false, true);
        }

        #endregion
    }
}
