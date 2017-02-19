using System.Collections.Generic;

namespace IsiiSports
{
    public class DeviceRegistration
    {
        public string Platform { get; set; }
        public string Handle { get; set; }
        public string[] Tags { get; set; }
    }

    public class NotificationPayload
    {
        public NotificationPayload()
        {
            Payload = new Dictionary<string, string>();
        }
        public string Action { get; set; }
        public Dictionary<string, string> Payload { get; set; }

    }

    public struct PushActions
    {
        public static string TeamCreated = "TeamCreated";
        public static string NewPlayerInTeam = "NewPlayerInTeam";
        public static string GameRevoked = "GameRevoked";
        public static string GameAccepted = "GameAccepted";
        public static string GameDeclined = "GameDeclined";
        public static string GameCompleted = "GameCompleted";
    }
}
