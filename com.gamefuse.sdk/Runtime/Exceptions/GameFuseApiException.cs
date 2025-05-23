using System;
using System.Net;

namespace GameFuse.Exceptions
{
    /// <summary>
    /// Exception thrown when a GameFuse API request fails.
    /// Contains information about the HTTP status code and API error details.
    /// </summary>
    public class GameFuseApiException : Exception
    {
        /// <summary>
        /// The HTTP status code returned by the API.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// The error code returned by the API, if available.
        /// </summary>
        public string ApiErrorCode { get; }

        /// <summary>
        /// Creates a new instance of GameFuseApiException.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="apiErrorCode">The API-specific error code, if available.</param>
        public GameFuseApiException(string message, HttpStatusCode statusCode, string apiErrorCode = null)
            : base(message)
        {
            StatusCode = statusCode;
            ApiErrorCode = apiErrorCode;
        }

        /// <summary>
        /// Creates a new instance of GameFuseApiException with an inner exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="apiErrorCode">The API-specific error code, if available.</param>
        public GameFuseApiException(string message, HttpStatusCode statusCode, Exception innerException, string apiErrorCode = null)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            ApiErrorCode = apiErrorCode;
        }

        /// <summary>
        /// Creates a new instance of GameFuseApiException for a generic API error.
        /// </summary>
        /// <param name="message">The error message.</param>
        public GameFuseApiException(string message)
            : base(message)
        {
            StatusCode = HttpStatusCode.InternalServerError;
        }

        /// <summary>
        /// Creates a new instance of GameFuseApiException for a client error.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public GameFuseApiException(string message, Exception innerException)
            : base(message, innerException)
        {
            StatusCode = HttpStatusCode.InternalServerError;
        }
    }

    /// <summary>
    /// Exception thrown when a user is not authenticated.
    /// </summary>
    public class GameFuseNotAuthenticatedException : GameFuseApiException
    {
        /// <summary>
        /// Creates a new instance of GameFuseNotAuthenticatedException.
        /// </summary>
        public GameFuseNotAuthenticatedException()
            : base("User is not authenticated. Please sign in first.", HttpStatusCode.Unauthorized)
        {
        }
    }

    /// <summary>
    /// Exception thrown when a API access is forbidden.
    /// </summary>
    public class GameFuseForbiddenException : GameFuseApiException
    {
        /// <summary>
        /// Creates a new instance of GameFuseForbiddenException.
        /// </summary>
        /// <param name="message">The error message.</param>
        public GameFuseForbiddenException(string message)
            : base(message, HttpStatusCode.Forbidden)
        {
        }
    }

    /// <summary>
    /// Exception thrown when a resource is not found.
    /// </summary>
    public class GameFuseNotFoundException : GameFuseApiException
    {
        /// <summary>
        /// Creates a new instance of GameFuseNotFoundException.
        /// </summary>
        /// <param name="message">The error message.</param>
        public GameFuseNotFoundException(string message)
            : base(message, HttpStatusCode.NotFound)
        {
        }
    }
}