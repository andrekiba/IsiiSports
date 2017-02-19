using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IsiiSports.DataObjects;
using IsiiSports.Interfaces;

namespace IsiiSports.Services.Stores
{
    public class PlayerStore : BaseStore<Player>, IPlayerStore
    {
        public override string Identifier => "Player";
    }
}
