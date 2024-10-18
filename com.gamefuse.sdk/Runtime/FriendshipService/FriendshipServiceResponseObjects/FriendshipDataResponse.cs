namespace GameFuseCSharp
{
    [System.Serializable]
    public class FriendshipDataResponse
    {
        public UserInfo[] friends;
        public FriendRequest[] outgoing_friend_requests;
        public FriendRequest[] incoming_friend_requests;
    }
}
