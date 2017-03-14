using System;

namespace IsiiSports.DataObjects
{
    public class Game : BaseDataObject
    {
		public string Name { get; set; }
		public string Description { get; set;}        
        public Team Team1 { get; set; }        
        public Team Team2 { get; set; }
        public GameField GameField { get; set; }
		public GameResult GameResult { get; set; }
        public DateTime GameDate { get; set; }
    }
}
