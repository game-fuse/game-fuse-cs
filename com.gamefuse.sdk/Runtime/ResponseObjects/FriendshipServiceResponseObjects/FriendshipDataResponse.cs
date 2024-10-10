using System;

namespace GameFuseCSharp
{
    [Serializable]
    public class FriendshipDataResponse
    {
        public UserInfo[] friends;
        public FriendRequest[] outgoingFriendRequests;
        public FriendRequest[] incomingFriendRequests;
    }
}
