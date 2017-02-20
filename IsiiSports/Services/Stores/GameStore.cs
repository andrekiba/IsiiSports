using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IsiiSports.DataObjects;
using IsiiSports.Interfaces;

namespace IsiiSports.Services.Stores
{
    public class GameStore : BaseStore<Game>, IGameStore
    {
        public override string Identifier => "Game";

        public override Task<IEnumerable<Game>> GetItemsAsync(bool forceRefresh = false)
        {
            var t1 = new Team { Name = "Informatici" };
            var t2 = new Team { Name = "Meccanici" };

            var list = new List<Game>()
            {
                new Game {Team1 = t1, Team2 = t2, GameDate = new DateTime(2017,2,20) },
                new Game {Team1 = t2, Team2 = t1, GameDate = new DateTime(2017,2,25) },
            };

            return Task.FromResult<IEnumerable<Game>>(list);
        }
    }
}
