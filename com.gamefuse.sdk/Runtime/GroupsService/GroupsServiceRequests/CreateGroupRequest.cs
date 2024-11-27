namespace GameFuseCSharp
{
    [System.Serializable]
    public class CreateGroupRequest
    {
        public string name;
        public string group_type;
        public int? max_group_size;
        public bool? can_auto_join;
        public bool? is_invite_only;
        public bool? searchable;
        public bool? admins_only_can_create_attributes;
    }
}
