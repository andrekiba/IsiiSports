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
    }
}
