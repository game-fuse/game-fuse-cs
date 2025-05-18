using System.Net;
using Newtonsoft.Json;

namespace GameFuse.Transport
{
    /// <summary>
    /// Represents an error response from the GameFuse API.
    /// </summary>
    internal class ApiErrorResponse
    {
        /// <summary>
        /// The error message returned by the API.
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// The error code returned by the API, if available.
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// Additional error details, if available.
        /// </summary>
        [JsonProperty("details")]
        public string Details { get; set; }

        /// <summary>
        /// Creates a user-friendly error message from the API error response.
        /// </summary>
        /// <returns>A user-friendly error message.</returns>
        public string ToUserFriendlyMessage()
        {
            if (!string.IsNullOrEmpty(Details))
            {
                return $"{Message}: {Details}";
            }
            return Message;
        }
    }

    /// <summary>
    /// Helper class for handling HTTP response status codes.
    /// </summary>
    internal static class HttpResponseHandler
    {
        /// <summary>
        /// Determines whether the HTTP status code indicates a successful response.
        /// </summary>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <returns>True if the status code indicates success, false otherwise.</returns>
        public static bool IsSuccessStatusCode(HttpStatusCode statusCode)
        {
            int code = (int)statusCode;
            return code >= 200 && code < 300;
        }

        /// <summary>
        /// Determines whether a failed request should be retried based on the status code.
        /// </summary>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <returns>True if the request should be retried, false otherwise.</returns>
        public static bool ShouldRetry(HttpStatusCode statusCode)
        {
            // Retry on server errors (5xx) and specific client errors
            switch (statusCode)
            {
                case HttpStatusCode.RequestTimeout:
                case HttpStatusCode.BadGateway:
                case HttpStatusCode.ServiceUnavailable:
                case HttpStatusCode.GatewayTimeout:
                    return true;
                default:
                    return (int)statusCode >= 500;
            }
        }
    }
}