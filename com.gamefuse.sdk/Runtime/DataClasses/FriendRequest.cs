using System;

namespace GameFuseCSharp
{
    [Serializable]
    public class FriendRequest : UserInfo
    {
        public int friendshipId;
        public string requestedAt;
    }
}
