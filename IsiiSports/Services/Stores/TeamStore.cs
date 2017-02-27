using System;
using System.Collections;
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

        public override Task<IEnumerable<Team>> GetItemsAsync(bool forceRefresh = false)
        {
            var teams = new List<Team>()
            {
                new Team() {Name = "Informatici"},
                new Team() {Name = "Meccanici"},
                new Team() {Name = "Elettronici"},
                new Team() {Name = "Chimici"},
                new Team() {Name = "Bidelli"},
                new Team() {Name = "Professori"}
            };
            return Task.FromResult<IEnumerable<Team>>(teams);
        }
    }
}
