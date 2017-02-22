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

        private const string AzureUserIdKey = "userId";
        private static readonly string AzureUserIdDefault = string.Empty;

        private const string AzureAuthTokenKey = "authToken";
        private static readonly string AzureAuthTokenDefault = string.Empty;

        #endregion


        public static string AzureAuthToken
        {
            get
            {
                return AppSettings.GetValueOrDefault(AzureAuthTokenKey, AzureAuthTokenDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(AzureAuthTokenKey, value);
            }
        }

        public static string AzureUserId
        {
            get
            {
                return AppSettings.GetValueOrDefault(AzureUserIdKey, AzureUserIdDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(AzureUserIdKey, value);
            }
        }

        public static bool IsLoggedIn
        {
            get
            {
                if (!AzureService.UseAuth)
                    return true;

                return !string.IsNullOrWhiteSpace(AzureUserId);
            }
        }

    }
}