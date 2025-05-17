using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using GameFuseCSharp;
using GameFuse.UIToolkit;
using GFuse = GameFuseCSharp.GameFuse; // Alias to avoid namespace conflict

/// <summary>
/// This class was previously used to add SignUpAsync functionality.
/// It is no longer needed as GameFuseCSharp.GameFuse now provides built-in async methods.
/// 
/// Please use GFuse.SignUpAsync(email, password, passwordConfirmation, username) directly
/// instead of this extension method.
/// 
/// This class is kept for backward compatibility but will be removed in a future version.
/// </summary>
[Obsolete("This extension is obsolete. Please use GFuse.SignUpAsync directly.", false)]
public static class GameFuseSignUpExtension
{
    /// <summary>
    /// Signs up a new user asynchronously. 
    /// This method is deprecated - use GFuse.SignUpAsync directly.
    /// </summary>
    /// <param name="user">The GameFuseUser class</param>
    /// <param name="email">The user's email</param>
    /// <param name="password">The user's password</param>
    /// <param name="passwordConfirmation">Password confirmation</param>
    /// <param name="username">The user's username</param>
    /// <returns>A task representing the async operation with SignInResponse</returns>
    [Obsolete("This method is obsolete. Please use GFuse.SignUpAsync directly.", false)]
    public static async Task<SignInResponse> SignUpAsync(this GameFuseUser user, string email, string password, string passwordConfirmation, string username)
    {
        // Use the modern GameFuse.SignUpAsync method which handles GameId and GameToken automatically
        return await GFuse.SignUpAsync(email, password, passwordConfirmation, username);
    }
}