# GameFuse SDK for Unity

GameFuse SDK provides Unity developers with a straightforward way to integrate GameFuse backend services into their games.

## Installation

### Option 1: Unity Package Manager (UPM)

1. Open your Unity project
2. Navigate to Window > Package Manager
3. Click the "+" button in the top-left corner
4. Select "Add package from git URL..."
5. Enter the repository URL: `https://github.com/gamefuse/unity-sdk.git`
6. Click "Add"

### Option 2: Manual Installation

1. Download the latest release from the [Releases](https://github.com/gamefuse/unity-sdk/releases) page
2. Extract the contents into your project's Assets folder

## Getting Started

### 1. Configure GameFuse Settings

Before using the SDK, you need to configure your GameFuse credentials:

1. In Unity, navigate to Assets > Create > GameFuse > Settings
2. This will create a GameFuseSettings asset in your project
3. Select the created asset and enter your Game ID and Game API Key in the Inspector

Alternatively, you can provide these credentials directly in your code when making API calls.

### 2. Authentication

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

### 3. Using the GameFuse API

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

## License

This SDK is distributed under the MIT license. See the [LICENSE](LICENSE) file for details.