using GameFuse.Exceptions;
using GameFuse.Transport;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace GameFuse.Tests.Common.Transport
{
    /// <summary>
    /// Mock implementation of ITransport for unit testing.
    /// </summary>
    public class MockTransport : ITransport
    {
        private readonly Dictionary<string, MockResponse> _responseMap = new Dictionary<string, MockResponse>();
        private Func<Dictionary<string, string>> _authHeaderProvider;

        /// <summary>
        /// Registers a mock response for a specific request.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="path">The API endpoint path.</param>
        /// <param name="responseJson">The JSON response to return.</param>
        /// <param name="statusCode">The HTTP status code to return.</param>
        public void RegisterResponse(string method, string path, string responseJson, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            string key = $"{method}:{path}";
            _responseMap[key] = new MockResponse
            {
                Json = responseJson,
                StatusCode = statusCode
            };
        }

        /// <summary>
        /// Registers a mock error response for a specific request.
        /// </summary>
        /// <param name="method">The HTTP method.</param>
        /// <param name="path">The API endpoint path.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="statusCode">The HTTP status code to return.</param>
        /// <param name="errorCode">The API error code, if any.</param>
        public void RegisterErrorResponse(string method, string path, string errorMessage, HttpStatusCode statusCode, string errorCode = null)
        {
            string key = $"{method}:{path}";
            _responseMap[key] = new MockResponse
            {
                Error = new MockErrorResponse
                {
                    Message = errorMessage,
                    Code = errorCode
                },
                StatusCode = statusCode
            };
        }

        /// <inheritdoc/>
        public Task<TResponse> GetAsync<TResponse>(string path, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default)
        {
            return SendMockRequestAsync<TResponse>("GET", path, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<TResponse> PostAsync<TRequest, TResponse>(string path, TRequest body, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default)
        {
            return SendMockRequestAsync<TResponse>("POST", path, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task PostAsync<TRequest>(string path, TRequest body, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default)
        {
            await SendMockRequestAsync<object>("POST", path, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<TResponse> PutAsync<TRequest, TResponse>(string path, TRequest body, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default)
        {
            return SendMockRequestAsync<TResponse>("PUT", path, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task PutAsync<TRequest>(string path, TRequest body, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default)
        {
            await SendMockRequestAsync<object>("PUT", path, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<TResponse> DeleteAsync<TResponse>(string path, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default)
        {
            return SendMockRequestAsync<TResponse>("DELETE", path, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(string path, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default)
        {
            await SendMockRequestAsync<object>("DELETE", path, cancellationToken);
        }

        /// <inheritdoc/>
        public void SetAuthHeaderProvider(Func<Dictionary<string, string>> authHeaderProvider)
        {
            _authHeaderProvider = authHeaderProvider;
        }

        /// <summary>
        /// Simulates sending a request and returning a mock response.
        /// </summary>
        /// <typeparam name="TResponse">The expected response type.</typeparam>
        /// <param name="method">The HTTP method.</param>
        /// <param name="path">The API endpoint path.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
        /// <returns>A task representing the asynchronous operation, containing the deserialized response.</returns>
        private async Task<TResponse> SendMockRequestAsync<TResponse>(string method, string path, CancellationToken cancellationToken)
        {
            // Simulate some delay to mimic network latency
            await Task.Delay(10, cancellationToken);

            string key = $"{method}:{path}";

            if (!_responseMap.TryGetValue(key, out MockResponse response))
            {
                throw new InvalidOperationException($"No mock response registered for {method} {path}");
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                if (response.Error != null)
                {
                    throw new GameFuseApiException(response.Error.Message, response.StatusCode, response.Error.Code);
                }

                throw new GameFuseApiException($"Mock request failed with status: {response.StatusCode}", response.StatusCode);
            }

            if (typeof(TResponse) == typeof(object))
            {
                return default;
            }

            return JsonConvert.DeserializeObject<TResponse>(response.Json);
        }

        /// <summary>
        /// Represents a mock HTTP response.
        /// </summary>
        private class MockResponse
        {
            /// <summary>
            /// The JSON response to return.
            /// </summary>
            public string Json { get; set; }

            /// <summary>
            /// The HTTP status code to return.
            /// </summary>
            public HttpStatusCode StatusCode { get; set; }

            /// <summary>
            /// The error response to return, if any.
            /// </summary>
            public MockErrorResponse Error { get; set; }
        }

        /// <summary>
        /// Represents a mock error response.
        /// </summary>
        private class MockErrorResponse
        {
            /// <summary>
            /// The error message.
            /// </summary>
            public string Message { get; set; }

            /// <summary>
            /// The API error code, if any.
            /// </summary>
            public string Code { get; set; }
        }
    }
}