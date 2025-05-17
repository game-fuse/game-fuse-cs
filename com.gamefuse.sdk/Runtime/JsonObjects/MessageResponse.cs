using System;
using Newtonsoft.Json;

namespace GameFuseCSharp
{
    /// <summary>
    /// Generic response object containing a message from the server.
    /// This is typically used for success or error messages.
    /// </summary>
    [Serializable]
    public class MessageResponse
    {
        /// <summary>
        /// The message returned by the server.
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
