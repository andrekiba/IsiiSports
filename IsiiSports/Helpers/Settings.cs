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

        private const string AuthProviderKey = "authProvider";
        private static readonly string AuthProviderDefault = string.Empty;
        private const string AzureUserIdKey = "azureUserId";
        private static readonly string AzureUserIdDefault = string.Empty;
        private const string AzureAuthTokenKey = "azureAuthToken";
        private static readonly string AzureAuthTokenDefault = string.Empty;
        private const string RefreshTokenKey = "refreshToken";
        private static readonly string RefreshTokenDefault = string.Empty;
        private const string AccessTokenKey = "accessToken";
        private static readonly string AccessTokenDefault = string.Empty;
        private const string UserIdKey = "userId";
        private static readonly string UserIdDefault = string.Empty;
        private const string PlayerIdKey = "playerId";
        private static readonly string PlayerIdDefault = string.Empty;

        #endregion

        public static string AuthProvider
        {
            get
            {
                return AppSettings.GetValueOrDefault(AuthProviderKey, AuthProviderDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(AuthProviderKey, value);
            }
        }
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
        public static string RefreshToken
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(RefreshTokenKey, RefreshTokenDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(RefreshTokenKey, value);
            }
        }
        public static string AccessToken
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(AccessTokenKey, AccessTokenDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(AccessTokenKey, value);
            }
        }
        public static string UserId
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(UserIdKey, UserIdDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(UserIdKey, value);
            }
        }
        public static string PlayerId
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(PlayerIdKey, PlayerIdDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(PlayerIdKey, value);
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