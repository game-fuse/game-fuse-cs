

namespace GameFuseCSharp
{
    public interface IServiceInitializable
    {
        void Initialize(string token, string name);
        void Initialize(string baseUrl, string token, string name);
    }
}
