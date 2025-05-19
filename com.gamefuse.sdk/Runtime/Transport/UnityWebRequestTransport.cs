using GameFuse.Config;
using GameFuse.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace GameFuse.Transport
{
    /// <summary>
    /// Implementation of ITransport that uses UnityWebRequest for HTTP communication.
    /// </summary>
    public class UnityWebRequestTransport : ITransport
    {
        private readonly string _baseUrl;
        private readonly int _maxRetryAttempts;
        private readonly int _requestTimeoutSeconds;
        private Func<Dictionary<string, string>> _authHeaderProvider;

        /// <summary>
        /// Creates a new instance of UnityWebRequestTransport.
        /// </summary>
        /// <param name="baseUrl">The base URL for API requests. If null, uses the value from GameFuseSettings.</param>
        /// <param name="maxRetryAttempts">Maximum number of retry attempts for failed requests. If null, uses the value from GameFuseSettings.</param>
        /// <param name="requestTimeoutSeconds">Timeout for requests in seconds. If null, uses the value from GameFuseSettings.</param>
        public UnityWebRequestTransport(string baseUrl = null, int? maxRetryAttempts = null, int? requestTimeoutSeconds = null)
        {
            GameFuseSettings settings = GameFuseSettings.Settings;

            _baseUrl = baseUrl ?? settings?.ApiBaseUrl ?? "https://gamefuse.co/api/v3";
            _maxRetryAttempts = maxRetryAttempts ?? settings?.MaxRetryAttempts ?? 3;
            _requestTimeoutSeconds = requestTimeoutSeconds ?? settings?.RequestTimeoutSeconds ?? 30;
        }

        /// <inheritdoc/>
        public Task<TResponse> GetAsync<TResponse>(string path, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<TResponse>(UnityWebRequest.kHttpVerbGET, path, null, headers, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<TResponse> PostAsync<TRequest, TResponse>(string path, TRequest body, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<TResponse>(UnityWebRequest.kHttpVerbPOST, path, body, headers, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task PostAsync<TRequest>(string path, TRequest body, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default)
        {
            await SendRequestAsync<EmptyResponse>(UnityWebRequest.kHttpVerbPOST, path, body, headers, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<TResponse> PutAsync<TRequest, TResponse>(string path, TRequest body, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<TResponse>(UnityWebRequest.kHttpVerbPUT, path, body, headers, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task PutAsync<TRequest>(string path, TRequest body, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default)
        {
            await SendRequestAsync<EmptyResponse>(UnityWebRequest.kHttpVerbPUT, path, body, headers, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<TResponse> DeleteAsync<TResponse>(string path, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<TResponse>(UnityWebRequest.kHttpVerbDELETE, path, null, headers, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<TResponse> DeleteAsync<TRequest, TResponse>(string path, TRequest body, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<TResponse>(UnityWebRequest.kHttpVerbDELETE, path, body, headers, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(string path, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default)
        {
            await SendRequestAsync<EmptyResponse>(UnityWebRequest.kHttpVerbDELETE, path, null, headers, cancellationToken);
        }

        /// <inheritdoc/>
        public void SetAuthHeaderProvider(Func<Dictionary<string, string>> authHeaderProvider)
        {
            _authHeaderProvider = authHeaderProvider;
        }

        /// <summary>
        /// Sends an HTTP request using UnityWebRequest with retry logic.
        /// </summary>
        /// <typeparam name="TResponse">The type of response expected.</typeparam>
        /// <param name="method">The HTTP method (GET, POST, PUT, DELETE).</param>
        /// <param name="path">The API endpoint path (relative to the base URL).</param>
        /// <param name="body">The request body object to be serialized (for POST and PUT).</param>
        /// <param name="headers">Headers to include in the request.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
        /// <returns>A task representing the asynchronous operation, containing the deserialized response.</returns>
        private async Task<TResponse> SendRequestAsync<TResponse>(string method, string path, object body = null, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default)
        {
            int retryCount = 0;
            TimeSpan retryDelay = TimeSpan.FromSeconds(1);

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    using (UnityWebRequest request = CreateRequest(method, path, body, headers))
                    {
                        // Set timeout
                        request.timeout = _requestTimeoutSeconds;

                        // Send the request
                        UnityWebRequestAsyncOperation operation = request.SendWebRequest();

                        // Wait for the request to complete
                        while (!operation.isDone)
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                request.Abort();
                                cancellationToken.ThrowIfCancellationRequested();
                            }
                            await Task.Yield();
                        }

                        // Check for network error
                        if (request.result == UnityWebRequest.Result.ConnectionError)
                        {
                            throw new GameFuseApiException($"Network error: {request.error}", HttpStatusCode.ServiceUnavailable);
                        }

                        // Get status code and check if we should retry
                        HttpStatusCode statusCode = (HttpStatusCode)request.responseCode;

                        if (!HttpResponseHandler.IsSuccessStatusCode(statusCode))
                        {
                            // Check if we should retry
                            if (retryCount < _maxRetryAttempts && HttpResponseHandler.ShouldRetry(statusCode))
                            {
                                retryCount++;
                                await Task.Delay(CalculateRetryDelay(retryCount), cancellationToken);
                                continue;
                            }

                            // Try to parse error response
                            string errorJson = request.downloadHandler.text;
                            ApiErrorResponse errorResponse = null;

                            try
                            {
                                errorResponse = JsonConvert.DeserializeObject<ApiErrorResponse>(errorJson);
                            }
                            catch (Exception)
                            {
                                // Ignore deserialization errors for error responses
                            }

                            string errorMessage = errorResponse?.ToUserFriendlyMessage() ?? $"Request failed with status: {statusCode}";

                            switch (statusCode)
                            {
                                case HttpStatusCode.Unauthorized:
                                    throw new GameFuseNotAuthenticatedException();
                                case HttpStatusCode.Forbidden:
                                    throw new GameFuseForbiddenException(errorMessage);
                                case HttpStatusCode.NotFound:
                                    throw new GameFuseNotFoundException(errorMessage);
                                default:
                                    throw new GameFuseApiException(errorMessage, statusCode, errorResponse?.Code);
                            }
                        }

                        // Parse response
                        string responseJson = request.downloadHandler.text;

                        if (typeof(TResponse) == typeof(EmptyResponse))
                        {
                            return (TResponse)(object)new EmptyResponse();
                        }
                        
                        return JsonConvert.DeserializeObject<TResponse>(responseJson);
                    }
                }
                catch (GameFuseApiException)
                {
                    // Rethrow API exceptions
                    throw;
                }
                catch (OperationCanceledException)
                {
                    // Rethrow cancellation exceptions
                    throw;
                }
                catch (Exception ex)
                {
                    // For other exceptions, retry if we haven't reached the limit
                    if (retryCount < _maxRetryAttempts)
                    {
                        retryCount++;
                        await Task.Delay(CalculateRetryDelay(retryCount), cancellationToken);
                    }
                    else
                    {
                        throw new GameFuseApiException($"Request failed after {_maxRetryAttempts} attempts: {ex.Message}", ex);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a UnityWebRequest with the specified parameters.
        /// </summary>
        /// <param name="method">The HTTP method (GET, POST, PUT, DELETE).</param>
        /// <param name="path">The API endpoint path (relative to the base URL).</param>
        /// <param name="body">The request body object to be serialized (for POST and PUT).</param>
        /// <param name="headers">Headers to include in the request.</param>
        /// <returns>A configured UnityWebRequest ready to be sent.</returns>
        private UnityWebRequest CreateRequest(string method, string path, object body, Dictionary<string, string> headers)
        {
            // Combine base URL and path
            string url = $"{_baseUrl.TrimEnd('/')}/{path.TrimStart('/')}";

            // Create request
            UnityWebRequest request = new UnityWebRequest(url, method);

            // Set up handlers
            request.downloadHandler = new DownloadHandlerBuffer();

            // Add body if needed (for POST and PUT)
            if (body != null)
            {
                string jsonBody = JsonConvert.SerializeObject(body);
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.SetRequestHeader("Content-Type", "application/json");
            }

            // Add authentication headers if available
            if (_authHeaderProvider != null)
            {
                Dictionary<string, string> authHeaders = _authHeaderProvider();
                if (authHeaders != null)
                {
                    foreach (var header in authHeaders)
                    {
                        Debug.Log($"Set request header {header.Key} : {header.Value}");
                        request.SetRequestHeader(header.Key, header.Value);
                    }
                }
            }

            // Add other headers
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.SetRequestHeader(header.Key, header.Value);
                }
            }

            return request;
        }

        /// <summary>
        /// Calculates the delay before retrying a failed request.
        /// Uses exponential backoff with jitter.
        /// </summary>
        /// <param name="retryCount">The current retry attempt.</param>
        /// <returns>The time to wait before the next retry.</returns>
        private TimeSpan CalculateRetryDelay(int retryCount)
        {
            // Exponential backoff: 2^retry * 100ms + random jitter
            int baseDelayMs = (int)Math.Pow(2, retryCount) * 100;
            int jitterMs = new System.Random().Next(0, 100);
            int delayMs = Math.Min(baseDelayMs + jitterMs, 10000); // Cap at 10 seconds
            return TimeSpan.FromMilliseconds(delayMs);
        }

        /// <summary>
        /// Empty response type for void methods.
        /// </summary>
        private class EmptyResponse { }
    }
}