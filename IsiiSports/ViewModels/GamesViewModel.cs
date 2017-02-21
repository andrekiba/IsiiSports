using IsiiSports.DataObjects;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IsiiSports.ViewModels
{
    public class GamesViewModel : BaseViewModel
    {
        public ObservableRangeCollection<Game> GameList { get; set; } = new ObservableRangeCollection<Game>();

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
    }
}
