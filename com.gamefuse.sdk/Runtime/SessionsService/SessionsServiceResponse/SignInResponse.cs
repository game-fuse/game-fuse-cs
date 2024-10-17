[System.Serializable]
public class SignInResponse
{
    public int id;
    public string username;
    public string email;
    public string display_email;
    public int credits;
    public int score;
    public string last_login;
    public int number_of_logins;
    public string authentication_token;
    public int events_total;
    public int events_current_month;
    public int game_sessions_total;
    public int game_sessions_current_month;
}