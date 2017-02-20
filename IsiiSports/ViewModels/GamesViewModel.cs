using IsiiSports.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IsiiSports.ViewModels
{
    public class GamesViewModel : BaseViewModel
    {
        public IEnumerable<Game> GameList { get; set; }

        public GamesViewModel()
        {

        }

        protected override async void ViewIsAppearing(object sender, EventArgs e)
        {
            if (this.GameList == null)
            {
                //TODO: Sto facendo await dentro ad un metodo che ritorna void
                this.GameList = await AzureService.GameStore.GetItemsAsync();
            }
        }
    }
}
