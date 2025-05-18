using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GameFuse.Transport
{
    /// <summary>
    /// Interface for the HTTP transport layer used by GameFuse SDK.
    /// </summary>
    public interface ITransport
    {
        /// <summary>
        /// Executes a GET request to the specified path.
        /// </summary>
        /// <typeparam name="TResponse">The expected response type.</typeparam>
        /// <param name="path">The API endpoint path (relative to the base URL).</param>
        /// <param name="headers">Optional headers to include in the request.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
        /// <returns>A task representing the asynchronous operation, containing the deserialized response.</returns>
        Task<TResponse> GetAsync<TResponse>(string path, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes a POST request to the specified path with the provided body.
        /// </summary>
        /// <typeparam name="TRequest">The request body type.</typeparam>
        /// <typeparam name="TResponse">The expected response type.</typeparam>
        /// <param name="path">The API endpoint path (relative to the base URL).</param>
        /// <param name="body">The request body object to be serialized.</param>
        /// <param name="headers">Optional headers to include in the request.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
        /// <returns>A task representing the asynchronous operation, containing the deserialized response.</returns>
        Task<TResponse> PostAsync<TRequest, TResponse>(string path, TRequest body, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes a POST request to the specified path with the provided body, expecting no response body.
        /// </summary>
        /// <typeparam name="TRequest">The request body type.</typeparam>
        /// <param name="path">The API endpoint path (relative to the base URL).</param>
        /// <param name="body">The request body object to be serialized.</param>
        /// <param name="headers">Optional headers to include in the request.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task PostAsync<TRequest>(string path, TRequest body, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes a PUT request to the specified path with the provided body.
        /// </summary>
        /// <typeparam name="TRequest">The request body type.</typeparam>
        /// <typeparam name="TResponse">The expected response type.</typeparam>
        /// <param name="path">The API endpoint path (relative to the base URL).</param>
        /// <param name="body">The request body object to be serialized.</param>
        /// <param name="headers">Optional headers to include in the request.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
        /// <returns>A task representing the asynchronous operation, containing the deserialized response.</returns>
        Task<TResponse> PutAsync<TRequest, TResponse>(string path, TRequest body, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes a PUT request to the specified path with the provided body, expecting no response body.
        /// </summary>
        /// <typeparam name="TRequest">The request body type.</typeparam>
        /// <param name="path">The API endpoint path (relative to the base URL).</param>
        /// <param name="body">The request body object to be serialized.</param>
        /// <param name="headers">Optional headers to include in the request.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task PutAsync<TRequest>(string path, TRequest body, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes a DELETE request to the specified path.
        /// </summary>
        /// <typeparam name="TResponse">The expected response type.</typeparam>
        /// <param name="path">The API endpoint path (relative to the base URL).</param>
        /// <param name="headers">Optional headers to include in the request.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
        /// <returns>A task representing the asynchronous operation, containing the deserialized response.</returns>
        Task<TResponse> DeleteAsync<TResponse>(string path, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes a DELETE request to the specified path, expecting no response body.
        /// </summary>
        /// <param name="path">The API endpoint path (relative to the base URL).</param>
        /// <param name="headers">Optional headers to include in the request.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteAsync(string path, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets a function that will be called to add authentication headers to requests.
        /// </summary>
        /// <param name="authHeaderProvider">A function that returns the authentication headers.</param>
        void SetAuthHeaderProvider(Func<Dictionary<string, string>> authHeaderProvider);
    }
}