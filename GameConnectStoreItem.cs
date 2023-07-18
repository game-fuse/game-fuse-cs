using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace GameConnectCSharp
{
    /// <summary>Class <c>GameConnectStoreItem</c> models a store item added 
    /// through the GameConnect web portal
    /// they can be retrieved and 'purchased' by GameConnectUsers through
    /// the API SDK, but not created.</summary>
    ///
    public class GameConnectStoreItem : MonoBehaviour
    {

        #region instance vars
        private string name;
        private string category;
        private string description;
        private int cost;
        private int id;
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

        #endregion

        #region constructor

        public GameConnectStoreItem(string name, string category, string description, int cost, int id)
        {
            this.name = name;
            this.category = category;
            this.description = description;
            this.cost = cost;
            this.id = id;

        }

        #endregion
    }



}

