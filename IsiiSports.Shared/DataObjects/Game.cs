using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsiiSports.DataObjects
{
    public class Game : BaseDataObject
    {
		public string Name { get; set; }
		public string Description { get; set;}

        public string Team1Id { get; set; }
        [ForeignKey("Team1Id")]
        public Team Team1 { get; set; }

        public string Team2Id { get; set; }
        [ForeignKey("Team2Id")]
        public Team Team2 { get; set; }

        public string GameFieldId { get; set; }
        [ForeignKey("GameFieldId")]
        public GameField GameField { get; set; }

        public string GameResultId { get; set; }
        [ForeignKey("GameFieldId")]
        public GameResult GameResult { get; set; }

        public DateTime GameDate { get; set; }
    }
}
