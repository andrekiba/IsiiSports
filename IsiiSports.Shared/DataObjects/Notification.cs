using System;

namespace IsiiSports.DataObjects
{
	public class Notification : BaseDataObject
	{
        public string Text { get; set; }
        public DateTime Date { get; set; }
    }
}
