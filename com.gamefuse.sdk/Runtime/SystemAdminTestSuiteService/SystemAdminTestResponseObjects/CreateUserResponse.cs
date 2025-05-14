using System;

namespace GameFuseCSharp
{
    [Serializable]
    public class CreateUserResponse
    {
        public int Id;
        public string Username;
        public string Email;
        public string DisplayEmail;
    }
}
