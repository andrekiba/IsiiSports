using IsiiSports.Services;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace IsiiSports.Helpers
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters. 
    /// </summary>
    public static class Settings
    {
        private static ISettings AppSettings => CrossSettings.Current;

        #region Setting Constants

        private const string UserIdKey = "userId";
        private static readonly string UserIdDefault = string.Empty;

        private const string AuthTokenKey = "authToken";
        private static readonly string AuthTokenDefault = string.Empty;

        #endregion


        public static string AuthToken
        {
            get
            {
                return AppSettings.GetValueOrDefault(AuthTokenKey, AuthTokenDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(AuthTokenKey, value);
            }
        }

        public static string UserId
        {
            get
            {
                return AppSettings.GetValueOrDefault(UserIdKey, UserIdDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(UserIdKey, value);
            }
        }

        public static bool IsLoggedIn
        {
            get
            {
                if (!AzureService.UseAuth)
                    return true;

                return !string.IsNullOrWhiteSpace(UserId);
            }
        }

    }
}