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
                new Player() {Name = "Mauro Icardi", Description = "Attaccante"},
                new Player() {Name = "Roberto Gagliardini", Description = "Attaccante"},
                new Player() {Name = "Ivan Perisic", Description = "Centrocampista"},
                new Player() {Name = "Rodrigo Palacio", Description = "Difensore"},
                new Player() {Name = "Samir Andanovic", Description = "Portiere"}
            };

            var playersMeccanici = new List<Player>()
            {
                new Player() {Name = "Gonzalo Higuain", Description = "Attaccante"},
                new Player() {Name = "Paulo Dybala", Description = "Centrocampista"},
                new Player() {Name = "Sami Khedira", Description = "Centrocampista"},
                new Player() {Name = "Leonardo Bonucci", Description = "Difensore"},
                new Player() {Name = "Gianluigi Buffon", Description = "Portiere"}
            };

            var playersChimici = new List<Player>()
            {
                new Player() {Name = "Marko Pjaca", Description = "Attaccante"},
                new Player() {Name = "Giorgio Chiellini", Description = "Centrocampista"},
                new Player() {Name = "Claudio Marchisio", Description = "Difensore"},
                new Player() {Name = "Stefano Sturaro", Description = "Difensore"},
                new Player() {Name = "Paolo De Ceglie", Description = "Portiere"}
            };

            var playersElettronici = new List<Player>()
            {
                new Player() {Name = "Jeison Murillo", Description = "Attaccante"},
                new Player() {Name = "Davide Santon", Description = "Centrocampista"},
                new Player() {Name = "Marco Andreolli", Description = "Difensore"},
                new Player() {Name = "Tommaso Berni", Description = "Difensore"},
                new Player() {Name = "Ionut Radu", Description = "Portiere"}
            };

            var playersProfessori = new List<Player>()
            {
                new Player() {Name = "Francesco Totti", Description = "Attaccante"},
                new Player() {Name = "Filippo Inzaghi", Description = "Attaccante"},
                new Player() {Name = "Alessandro Del Piero", Description = "Centrocampista"},
                new Player() {Name = "Fabio Cannavaro", Description = "Difensore"},
                new Player() {Name = "Angelo Peruzzi", Description = "Portiere"}
            };

            var playersBidelli = new List<Player>()
            {
                new Player() {Name = "Manuel Locatelli", Description = "Attaccante"},
                new Player() {Name = "Carlos Bacca", Description = "Centrocampista"},
                new Player() {Name = "Ignazio Abate", Description = "Centrocampista"},
                new Player() {Name = "Riccardo Montolivo", Description = "Difensore"},
                new Player() {Name = "Alessandro Plizzari", Description = "Portiere"}
            };

            var teams = new List<Team>()
            {
                new Team() { Name = "Informatici", Players = playersInformatici,Description = "Squadra fortissima"},
                new Team() { Name = "Meccanici", Players = playersMeccanici, Description = "Squadra forte"},
                new Team() { Name = "Elettronici", Players = playersElettronici, Description = "Squadra Debole"},
                new Team() { Name = "Chimici", Players = playersChimici, Description = "Squadra nuova"},
                new Team() { Name = "Bidelli", Players = playersBidelli, Description = "Squadra normale"},
                new Team() { Name = "Professori", Players = playersProfessori, Description = "Squadra nuova"}
            };
            return Task.FromResult<IEnumerable<Team>>(teams);
        }
    }
}
