﻿namespace IsiiSports.DataObjects
{
	public class GameResult : BaseDataObject
	{
		public int Index { get; set; }
		public int? Team1Score { get; set; }
		public int? Team2Score { get; set; }
	    public string GameId { get; set; }
	    public Game Game { get; set; }
    }
}
