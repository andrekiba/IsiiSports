using System.Collections.Generic;

namespace IsiiSports.DataObjects
{
    public class Team : BaseDataObject
    {
		public string Name { get; set;}
		public IList<Player> Players { get; set; } = new List<Player>();
		public IList<Game> Games { get; set; } = new List<Game>(); 
		public string NotificationId { get; set; }
		public string ImageUrl { get; set;}
    }
}
