using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IsiiSports.DataObjects;
using IsiiSports.Interfaces;

namespace IsiiSports.Services.Stores
{
    public class TeamStore : BaseStore<Team>, ITeamStore
    {
        public override string Identifier => "Team";
    }
}
