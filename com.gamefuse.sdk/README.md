# GameFuse SDK for Unity

#### Sign Up a New User

```csharp
try
{
    // Using the configured settings
    GameFuseUser user = await GameFuseUser.SignUpAsync("user@example.com", "password123", "username");
    
    // Or provide credentials manually
    // GameFuseUser user = await GameFuseUser.SignUpAsync("user@example.com", "password123", "username", "your-game-id", "your-game-api-key");
    
    Debug.Log($"User signed up successfully! User ID: {user.Id}");
}
catch (GameFuseApiException ex)
{
    Debug.LogError($"Failed to sign up: {ex.Message}");
}
```

#### Sign In an Existing User

```csharp
try
{
    // Using the configured settings
    GameFuseUser user = await GameFuseUser.SignInAsync("username", "password123");
    
    // Or provide credentials manually
    // GameFuseUser user = await GameFuseUser.SignInAsync("username", "password123", "your-game-id", "your-game-api-key");
    
    Debug.Log($"User signed in successfully! User ID: {user.Id}");
}
catch (GameFuseApiException ex)
{
    Debug.LogError($"Failed to sign in: {ex.Message}");
}
```

### Using the GameFuse API

After authentication, you can access all GameFuse features through the `GameFuseUser` facade:

```csharp
// Get user profile
var currentUser = await GameFuseUser.GetCurrentUserAsync();

// Get store items
var storeItems = await GameFuseUser.GetStoreItemsAsync();

// Get user's credit balance
var creditBalance = await GameFuseUser.GetCreditBalanceAsync();

// Get user's game rounds
var gameRounds = await GameFuseUser.GetCurrentUserGameRoundsAsync();

// Get user's friends
var friends = await GameFuseUser.GetFriendsAsync();
```

## Sample UI

The SDK includes a sample UI implementation to help you get started. To use it:

1. In the Package Manager, select the GameFuse SDK package
2. Click the "Import" button next to "GameFuse Sample UI" in the Samples section
3. This will import the sample UI files into your project's Assets folder
4. Add the sample UI components to your scene

## API Documentation

For detailed API documentation, see the [API Reference](https://docs.gamefuse.co/api).
