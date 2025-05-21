using Newtonsoft.Json;
using System.Collections.Generic;

namespace GameFuse.Models.Shared
{
    /// <summary>
    /// Represents the type of a game round.
    /// </summary>
    public enum GameRoundType
    {
        /// <summary>
        /// A single-player game round.
        /// </summary>
        SinglePlayer,
        
        /// <summary>
        /// A multiplayer game round.
        /// </summary>
        Multiplayer
    }

    /// <summary>
    /// Consolidated model that represents a game round in GameFuse.
    /// Covers both single-player and multiplayer use cases.
    /// </summary>
    public class GameRound
    {
        /// <summary>
        /// The game round's unique identifier.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; internal set; }

        /// <summary>
        /// The ID of the user who created the game round.
        /// </summary>
        [JsonProperty("game_user_id")]
        public int GameUserId { get; internal set; }

        /// <summary>
        /// The start time of the game round.
        /// </summary>
        [JsonProperty("start_time")]
        public string StartTime { get; internal set; }

        /// <summary>
        /// The end time of the game round.
        /// </summary>
        [JsonProperty("end_time")]
        public string EndTime { get; internal set; }

        /// <summary>
        /// The score achieved in the game round.
        /// </summary>
        [JsonProperty("score")]
        public double Score { get; internal set; }

        /// <summary>
        /// The place finished in the game round.
        /// </summary>
        [JsonProperty("place")]
        public int Place { get; internal set; }

        /// <summary>
        /// The type of game played.
        /// </summary>
        [JsonProperty("game_type")]
        public string GameType { get; internal set; }

        /// <summary>
        /// The ID of the associated multiplayer game round, if applicable.
        /// </summary>
        [JsonProperty("multiplayer_game_round_id")]
        public int? MultiplayerGameRoundId { get; internal set; }

        /// <summary>
        /// Additional metadata related to the game round.
        /// </summary>
        [JsonProperty("metadata")]
        public Dictionary<string, object> Metadata { get; internal set; }

        /// <summary>
        /// Rankings of all participants in a multiplayer game round, if applicable.
        /// </summary>
        [JsonProperty("rankings")]
        public List<RankingEntry> Rankings { get; internal set; }

        /// <summary>
        /// The date and time when the game round was created.
        /// </summary>
        [JsonProperty("created_at")]
        public string CreatedAt { get; internal set; }

        /// <summary>
        /// The date and time when the game round was last updated.
        /// </summary>
        [JsonProperty("updated_at")]
        public string UpdatedAt { get; internal set; }

        /// <summary>
        /// Gets the type of the game round.
        /// </summary>
        [JsonIgnore]
        public GameRoundType RoundType
        {
            get
            {
                return MultiplayerGameRoundId.HasValue || 
                       (Rankings != null && Rankings.Count > 0) 
                       ? GameRoundType.Multiplayer 
                       : GameRoundType.SinglePlayer;
            }
        }

        /// <summary>
        /// Creates a new GameRound instance with empty collections.
        /// </summary>
        public GameRound()
        {
            Metadata = new Dictionary<string, object>();
            Rankings = new List<RankingEntry>();
        }
    }

    /// <summary>
    /// Represents a player's ranking entry in a multiplayer game round.
    /// </summary>
    public class RankingEntry
    {
        /// <summary>
        /// The unique identifier of the game round for this ranking.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; internal set; }
        
        /// <summary>
        /// The place finished in the game round.
        /// </summary>
        [JsonProperty("place")]
        public int Place { get; internal set; }

        /// <summary>
        /// The score achieved in the game round.
        /// </summary>
        [JsonProperty("score")]
        public double Score { get; internal set; }

        /// <summary>
        /// The start time of the game round.
        /// </summary>
        [JsonProperty("start_time")]
        public string StartTime { get; internal set; }

        /// <summary>
        /// The end time of the game round.
        /// </summary>
        [JsonProperty("end_time")]
        public string EndTime { get; internal set; }

        /// <summary>
        /// The user associated with this ranking entry.
        /// </summary>
        [JsonProperty("user")]
        public UserRankInfo User { get; internal set; }
    }

    /// <summary>
    /// Represents user information in a ranking entry.
    /// </summary>
    public class UserRankInfo
    {
        /// <summary>
        /// The user's ID.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; internal set; }

        /// <summary>
        /// The user's username.
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; internal set; }

        /// <summary>
        /// The user's email.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; internal set; }

        /// <summary>
        /// The display email for the user.
        /// </summary>
        [JsonProperty("display_email", NullValueHandling = NullValueHandling.Ignore)]
        public string? DisplayEmail { get; internal set; }

        /// <summary>
        /// The user's credits.
        /// </summary>
        [JsonProperty("credits")]
        public int Credits { get; internal set; }

        /// <summary>
        /// The user's score.
        /// </summary>
        [JsonProperty("score")]
        public double Score { get; internal set; }

        /// <summary>
        /// Indicates if this is a new user.
        /// </summary>
        [JsonProperty("is_new_user")]
        public bool IsNewUser { get; internal set; }
    }

    /// <summary>
    /// Container for a list of game rounds from API response.
    /// </summary>
    public class GameRoundListResponse
    {
        /// <summary>
        /// List of game rounds.
        /// </summary>
        [JsonProperty("game_rounds")]
        public List<GameRound> GameRounds { get; set; }

        /// <summary>
        /// Creates a new GameRoundListResponse with an empty list.
        /// </summary>
        public GameRoundListResponse()
        {
            GameRounds = new List<GameRound>();
        }
    }

    /// <summary>
    /// Response for delete operation.
    /// </summary>
    public class GameRoundDeleteResponse
    {
        /// <summary>
        /// Success message.
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }
    }

    /// <summary>
    /// Represents a leaderboard entry.
    /// </summary>
    public class LeaderboardEntry
    {
        /// <summary>
        /// The user's unique identifier.
        /// </summary>
        [JsonProperty("user_id")]
        public int UserId { get; internal set; }

        /// <summary>
        /// The user's display username.
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; internal set; }

        /// <summary>
        /// The user's score.
        /// </summary>
        [JsonProperty("score")]
        public double Score { get; internal set; }

        /// <summary>
        /// The user's rank in the leaderboard.
        /// </summary>
        [JsonProperty("rank")]
        public int Rank { get; internal set; }

        /// <summary>
        /// Custom data associated with the leaderboard entry.
        /// </summary>
        [JsonProperty("custom_data")]
        public string CustomData { get; internal set; }

        /// <summary>
        /// When the leaderboard entry was created.
        /// </summary>
        [JsonProperty("created_at")]
        public string CreatedAt { get; internal set; }

        /// <summary>
        /// The ID of the game round associated with this entry.
        /// </summary>
        [JsonProperty("game_round_id")]
        public int GameRoundId { get; internal set; }
    }

    /// <summary>
    /// Container for a list of leaderboard entries.
    /// </summary>
    public class LeaderboardEntries
    {
        /// <summary>
        /// The list of leaderboard entries.
        /// </summary>
        [JsonProperty("leaderboard_entries")]
        public List<LeaderboardEntry> Entries { get; set; }

        /// <summary>
        /// Creates a new LeaderboardEntries instance with an empty list.
        /// </summary>
        public LeaderboardEntries()
        {
            Entries = new List<LeaderboardEntry>();
        }
    }

    /// <summary>
    /// Represents an entry in a named leaderboard.
    /// </summary>
    public class LeaderboardEntryModel
    {
        /// <summary>
        /// User's display username.
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; internal set; }

        /// <summary>
        /// Score for the leaderboard entry.
        /// </summary>
        [JsonProperty("score")]
        public double Score { get; internal set; }

        /// <summary>
        /// Name of the leaderboard within the game.
        /// </summary>
        [JsonProperty("leaderboard_name")]
        public string LeaderboardName { get; internal set; }

        /// <summary>
        /// The user ID associated with this leaderboard entry.
        /// </summary>
        [JsonProperty("game_user_id")]
        public int GameUserId { get; internal set; }

        /// <summary>
        /// Additional metadata related to the leaderboard entry.
        /// </summary>
        [JsonProperty("metadata")]
        public object Metadata { get; internal set; }

        /// <summary>
        /// When the leaderboard entry was created.
        /// </summary>
        [JsonProperty("created_at")]
        public string CreatedAt { get; internal set; }
    }

    /// <summary>
    /// Container for a list of leaderboard entries.
    /// </summary>
    public class LeaderboardEntriesResponse
    {
        /// <summary>
        /// List of leaderboard entries.
        /// </summary>
        [JsonProperty("leaderboard_entries")]
        public List<LeaderboardEntryModel> LeaderboardEntries { get; internal set; }

        /// <summary>
        /// Creates a new LeaderboardEntriesResponse with an empty list.
        /// </summary>
        public LeaderboardEntriesResponse()
        {
            LeaderboardEntries = new List<LeaderboardEntryModel>();
        }
    }
}