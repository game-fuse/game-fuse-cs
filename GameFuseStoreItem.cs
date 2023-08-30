using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace GameFuseCSharp
{
    /// <summary>Class <c>GameFuseStoreItem</c> models a store item added 
    /// through the GameFuse web portal
    /// they can be retrieved and 'purchased' by GameFuseUsers through
    /// the API SDK, but not created.</summary>
    ///
    public class GameFuseStoreItem : MonoBehaviour
    {

        #region instance vars
        private string name;
        private string category;
        private string description;
        private int cost;
        private int id;
        private string icon_url;
        #endregion

        #region instance getters
        public string GetName()
        {
            return name;
        }
        public string GetCategory()
        {
            return category;
        }
        public string GetDescription()
        {
            return description;
        }
        public int GetCost()
        {
            return cost;
        }
        public int GetId()
        {
            return id;
        }
        public string GetIconUrl()
        {
            return icon_url;
        }

        #endregion

        #region constructor

        public GameFuseStoreItem(string name, string category, string description, int cost, int id, string icon_url)
        {
            this.name = name;
            this.category = category;
            this.description = description;
            this.cost = cost;
            this.id = id;
            this.icon_url = icon_url;
        }

        #endregion
    }



}

