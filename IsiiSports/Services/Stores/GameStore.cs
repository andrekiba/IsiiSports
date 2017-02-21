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
            var t3 = new Team { Name = "Professori Isii" };
            var t4 = new Team { Name = "Professori Poli Milano" };
            var g1 = new GameField() { Name = "Palestra 1" };
            var g2 = new GameField() { Name = "Palestra 2" };

            var list = new List<Game>()
            {
                new Game {Team1 = t1, Team2 = t2, GameDate = new DateTime(2017,2,20,16,30,0), GameField = g1 },
                new Game {Team1 = t2, Team2 = t1, GameDate = new DateTime(2017,2,25,17,0,0), GameField = g2},
                new Game {Team1 = t3, Team2 = t4, GameDate = new DateTime(2017,2,20,16,30,0), GameField = g1 },
                new Game {Team1 = t2, Team2 = t1, GameDate = new DateTime(2017,2,25,17,0,0), GameField = g2},
                new Game {Team1 = t1, Team2 = t2, GameDate = new DateTime(2017,2,20,16,30,0), GameField = g1 },
                new Game {Team1 = t2, Team2 = t1, GameDate = new DateTime(2017,2,25,17,0,0), GameField = g2},
                new Game {Team1 = t1, Team2 = t2, GameDate = new DateTime(2017,2,20,16,30,0), GameField = g1 },
                new Game {Team1 = t2, Team2 = t1, GameDate = new DateTime(2017,2,25,17,0,0), GameField = g2},
                new Game {Team1 = t1, Team2 = t2, GameDate = new DateTime(2017,2,20,16,30,0), GameField = g1 },
                new Game {Team1 = t2, Team2 = t1, GameDate = new DateTime(2017,2,25,17,0,0), GameField = g2},
                new Game {Team1 = t1, Team2 = t2, GameDate = new DateTime(2017,2,20,16,30,0), GameField = g1 },
                new Game {Team1 = t2, Team2 = t1, GameDate = new DateTime(2017,2,25,17,0,0), GameField = g2},
            };

            return Task.FromResult<IEnumerable<Game>>(list);
        }
    }
}
