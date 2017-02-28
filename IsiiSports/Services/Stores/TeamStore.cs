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
            var playersInformatici = new List<Player>()
            {
                new Player() {Name = "Mauro Icardi"},
                new Player() {Name = "Roberto Gagliardini"},
                new Player() {Name = "Ivan Perisic"},
                new Player() {Name = "Rodrigo Palacio"},
                new Player() {Name = "Samir Andanovic"}
            };

            var playersMeccanici = new List<Player>()
            {
                new Player() {Name = "Gonzalo Higuain"},
                new Player() {Name = "Paulo Dybala"},
                new Player() {Name = "Sami Khedira"},
                new Player() {Name = "Leonardo Bonucci"},
                new Player() {Name = "Gianluigi Buffon"}
            };

            var playersChimici = new List<Player>()
            {
                new Player() {Name = "Marko Pjaca"},
                new Player() {Name = "Giorgio Chiellini"},
                new Player() {Name = "Claudio Marchisio"},
                new Player() {Name = "Stefano Sturaro"},
                new Player() {Name = "Paolo De Ceglie"}
            };

            var playersElettronici = new List<Player>()
            {
                new Player() {Name = "Jeison Murillo"},
                new Player() {Name = "Davide Santon"},
                new Player() {Name = "Marco Andreolli"},
                new Player() {Name = "Tommaso Berni"},
                new Player() {Name = "Ionut Radu"}
            };

            var playersProfessori = new List<Player>()
            {
                new Player() {Name = "Francesco Totti"},
                new Player() {Name = "Filippo Inzaghi"},
                new Player() {Name = "Alessandro Del Piero"},
                new Player() {Name = "Fabio Cannavaro"},
                new Player() {Name = "Angelo Peruzzi"}
            };

            var playersBidelli = new List<Player>()
            {
                new Player() {Name = "Manuel Locatelli"},
                new Player() {Name = "Carlos Bacca"},
                new Player() {Name = "Ignazio Abate"},
                new Player() {Name = "Riccardo Montolivo"},
                new Player() {Name = "Alessandro Plizzari"}
            };

            var teams = new List<Team>()
            {
                new Team() { Name = "Informatici", Players = playersInformatici},
                new Team() { Name = "Meccanici", Players = playersMeccanici},
                new Team() { Name = "Elettronici", Players = playersElettronici},
                new Team() { Name = "Chimici", Players = playersChimici},
                new Team() { Name = "Bidelli", Players = playersBidelli},
                new Team() { Name = "Professori", Players = playersProfessori}
            };
            return Task.FromResult<IEnumerable<Team>>(teams);
        }
    }
}
