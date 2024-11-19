
namespace GameFuseCSharp
{
    public class ApiException : System.Exception
    {
        public long StatusCode { get; }
        public string ResponseBody { get; }

        public ApiException(long statusCode, string message, string responseBody) : base(message)
        {
            StatusCode = statusCode;
            ResponseBody = responseBody;
        }
    }
}
