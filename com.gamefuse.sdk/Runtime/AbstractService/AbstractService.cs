using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GameFuseCSharp
{
    public abstract class AbstractService
    {
        protected string _baseUrl;
        protected string _token;
        protected const int TimeoutSeconds = 10;

        // Add static JsonSerializerSettings to ensure consistent serialization across the service
        protected static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            },
            NullValueHandling = NullValueHandling.Ignore
        };

        protected enum HttpVerbs
        {
            DELETE,
            GET,
            POST,
            PUT
        }

        protected UnityWebRequest CreateRequest(string url, HttpVerbs method, string jsonBody)
        {
            UnityWebRequest webRequest = CreateRequest(url, method);
            SetRequestBody(webRequest, jsonBody);
            return webRequest;
        }

        protected UnityWebRequest CreateRequest(string url, HttpVerbs method)
        {
            UnityWebRequest webRequest = new UnityWebRequest(url, method.ToString());
            SetRequestHeaders(webRequest);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.timeout = TimeoutSeconds;
            return webRequest;
        }

        protected virtual void SetRequestHeaders(UnityWebRequest webRequest)
        {
            webRequest.SetRequestHeader("authentication-token", _token);
            webRequest.SetRequestHeader("Content-Type", "application/json");
        }

        protected void SetRequestBody(UnityWebRequest webRequest, string jsonBody)
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        }

        protected async Task<T> SendRequestAsync<T>(UnityWebRequest webRequest) where T : class, new()
        {
            try
            {
                using (webRequest)
                {
                    var operation = webRequest.SendWebRequest();

                    while (!operation.isDone)
                    {
                        await Task.Yield();
                    }

                    switch (webRequest.result)
                    {
                        case UnityWebRequest.Result.Success:
                            string jsonResponse = webRequest.downloadHandler.text;
                            try
                            {
                                return JsonConvert.DeserializeObject<T>(jsonResponse, JsonSettings);
                            }
                            catch (JsonException ex)
                            {
                                throw new ApiException(
                                    0,
                                    $"Failed to deserialize response: {ex.Message}",
                                    jsonResponse
                                );
                            }

                        case UnityWebRequest.Result.ConnectionError:
                        case UnityWebRequest.Result.ProtocolError:
                        case UnityWebRequest.Result.DataProcessingError:
                            throw new ApiException(
                                webRequest.responseCode,
                                $"{webRequest.result}: {webRequest.error}",
                                webRequest.downloadHandler?.text ?? string.Empty
                            );

                        default:
                            throw new ApiException(
                                0,
                                $"Unexpected error: {webRequest.result}",
                                webRequest.downloadHandler?.text ?? string.Empty
                            );
                    }
                }
            }
            catch (ApiException)
            {
                // Re-throw ApiException as it already contains all the necessary information
                throw;
            }
            catch (Exception ex)
            {
                // Wrap any other exceptions in an ApiException
                throw new ApiException(0, $"Unexpected error: {ex.Message}", string.Empty);
            }
        }

        protected string SerializeRequest<T>(T request) where T : class
        {
            try
            {
                return JsonConvert.SerializeObject(request, JsonSettings);
            }
            catch (JsonException ex)
            {
                throw new ApiException(
                    0,
                    $"Failed to serialize request: {ex.Message}",
                    string.Empty
                );
            }
        }
    }
}
