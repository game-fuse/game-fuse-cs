using System;

namespace GameFuseCSharp
{
    [Serializable]
    public class CreateStoreItemRequest
    {
        public int game_id;
        public string name;
        public string description;
        public string category;
        public int cost; 
    }
}
