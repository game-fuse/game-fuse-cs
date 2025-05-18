using UnityEngine;

namespace GameFuse.Config
{
    /// <summary>
    /// ScriptableObject for storing GameFuse configuration settings.
    /// This stores static, game-wide configuration like GameId and GameApiKey.
    /// </summary>
    [CreateAssetMenu(fileName = "GameFuseSettings", menuName = "GameFuse/Settings", order = 1)]
    public class GameFuseSettings : ScriptableObject
    {
        /// <summary>
        /// The unique identifier for your game in the GameFuse system.
        /// </summary>
        [Tooltip("The unique identifier for your game in the GameFuse system")]
        public string GameId;

        /// <summary>
        /// API key specific to your game, used primarily for sign-up/sign-in.
        /// </summary>
        [Tooltip("API key specific to your game, used primarily for sign-up/sign-in")]
        public string GameApiKey;

        /// <summary>
        /// Base URL for the GameFuse API. Change only if you're using a custom API endpoint.
        /// </summary>
        [Tooltip("Base URL for the GameFuse API. Change only if you're using a custom API endpoint")]
        public string ApiBaseUrl = "https://api.gamefuse.co/api/v3";

        /// <summary>
        /// Maximum number of retry attempts for failed API requests.
        /// </summary>
        [Tooltip("Maximum number of retry attempts for failed API requests")]
        [Range(0, 5)]
        public int MaxRetryAttempts = 3;

        /// <summary>
        /// Timeout for API requests in seconds.
        /// </summary>
        [Tooltip("Timeout for API requests in seconds")]
        [Range(5, 60)]
        public int RequestTimeoutSeconds = 30;

        private static GameFuseSettings _settings;

        /// <summary>
        /// Access the GameFuseSettings instance loaded from Resources.
        /// </summary>
        public static GameFuseSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = Resources.Load<GameFuseSettings>(nameof(GameFuseSettings));
                    if (_settings == null)
                    {
                        Debug.LogError($"GameFuseSettings ScriptableObject not found in a Resources folder. Please create one via Assets > Create > GameFuse > Settings and populate GameId and GameApiKey.");
                    }
                }
                return _settings;
            }
        }
    }
}