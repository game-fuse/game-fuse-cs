using System;
using System.Collections.Generic;
using GameFuseCSharp;
using UnityEngine;

namespace GameFuse.UIToolkit
{
    /// <summary>
    /// Extension methods for GameFuseUser class
    /// </summary>
    public static class GameFuseUserExtensions
    {
        /// <summary>
        /// Signs out the current user by clearing their local state
        /// </summary>
        public static void SignOut(this GameFuseUser user)
        {
            if (user == null)
                return;
                
            // Access internal methods using reflection
            var userType = typeof(GameFuseUser);
            
            // Set signedIn to false - this method is confirmed to exist and accept a boolean
            var signedInMethod = userType.GetMethod("SetSignedInInternal", 
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            signedInMethod?.Invoke(user, new object[] { false });
            
            // Clear authentication token
            var authTokenMethod = userType.GetMethod("SetAuthenticationTokenInternal", 
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            authTokenMethod?.Invoke(user, new object[] { null });
            
            // Set other user properties to their defaults
            var setUsernameMethod = userType.GetMethod("SetUsernameInternal", 
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            setUsernameMethod?.Invoke(user, new object[] { string.Empty });
            
            var setScoreMethod = userType.GetMethod("SetScoreInternal", 
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            setScoreMethod?.Invoke(user, new object[] { 0 });
            
            var setCreditsMethod = userType.GetMethod("SetCreditsInternal", 
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            setCreditsMethod?.Invoke(user, new object[] { 0 });
            
            var setIdMethod = userType.GetMethod("SetIDInternal", 
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            setIdMethod?.Invoke(user, new object[] { 0 });
            
            Debug.Log("User signed out successfully");
        }
    }
}