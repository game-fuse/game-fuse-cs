namespace GameFuseCSharp
{
    [System.Serializable]
    public class SignUpRequest
    {
        public string email;
        public string password;
        public string password_confirmation;
        public string username;
        public int game_id;
        public string game_token;
    }
}
