namespace GameFuseCSharp
{
    [System.Serializable]
    public class GroupResponse
    {
        public int id;
        public string name;
        public string group_type;
        public bool can_auto_join;
        public bool is_invite_only;
        public int max_group_size;
        public bool searchable;
        public int member_count;
        public UserInfo[] members;
        public UserInfo[] admins;
        public object[] join_requests;
        public object[] invites;
    }
}
