# GameFuse UI Toolkit Example

This example demonstrates how to use the GameFuse SDK with Unity's UI Toolkit.

## Setup Instructions

### 1. Create a GameFuse Config Asset

The UI Toolkit example uses a ScriptableObject to store your GameFuse game ID and token. To create this asset:

1. In the Unity Editor, right-click in the Project window
2. Select **Create > GameFuse > Config**
3. Name the asset (e.g., "GameFuseConfig")
4. In the Inspector, enter your GameFuse Game ID and Game Token

### 2. Assign the Config to the Demo Panel

1. Select the GameFuseDemoPanel GameObject in the Hierarchy
2. In the Inspector, find the "GameFuseDemoPanelController" component
3. Drag your created GameFuseConfig asset to the "Config" field

### 3. Run the Example

When you run the example, it will automatically initialize the GameFuse SDK using the credentials from the config asset.

## Manual Setup

If you prefer not to use the config asset, you can also manually enter your Game ID and Game Token in the UI at runtime.

## Features Demonstrated

- Game setup and initialization
- User registration and authentication
- Friends management
- Leaderboards
- Chat functionality
- Game variables
- Store items

## Troubleshooting

If you encounter issues:

1. Check that your Game ID and Game Token are correct
2. Ensure you have an active internet connection
3. Check the Console for any error messages

For more help, visit the [GameFuse Documentation](https://docs.gamefuse.co/).