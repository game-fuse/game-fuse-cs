using System;

namespace GameFuseCSharp
{
    [Serializable]
    public class CreateUserRequest
    {
        public int game_id;
        public string username;
        public string email;
    }
    
}
