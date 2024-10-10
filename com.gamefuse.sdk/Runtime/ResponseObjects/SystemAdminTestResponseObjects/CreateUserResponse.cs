using System;

namespace GameFuseCSharp
{
    [Serializable]
    public class CreateUserResponse
    {
        public int id;
        public string username;
        public string email;
        public string display_email;
    }
}
