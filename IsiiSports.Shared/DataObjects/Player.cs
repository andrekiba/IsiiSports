﻿namespace IsiiSports.DataObjects
{
    public class Player : BaseDataObject
    {
		public string Name { get; set; }
		public string Nickname { get; set; }
		public string Email { get; set; }
		public string ProfileImageUrl { get; set; }
		public string AuthToken { get; set; }
        public bool IsAdmin { get; set; }
        public string NotificationId { get; set; }
        public string TeamId { get; set; }
        public Team Team { get; set; }
    }
}
