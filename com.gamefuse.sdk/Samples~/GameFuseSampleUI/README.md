# GameFuse Sample UI

This sample demonstrates how to integrate the GameFuse SDK into a Unity project using UI Toolkit. It provides a complete UI for authentication, user management, and interacting with GameFuse services.

## Features

- Authentication UI (Sign In, Sign Up, Forgot Password)
- User profile display
- Store items browsing
- Friends list
- Leaderboard display

## Setting Up the Sample

1. Import this sample into your Unity project via Package Manager.
2. Create a new scene or use an existing one.
3. Add the following prefab to your scene:
   - GameFuseSample

4. Make sure you have created and configured a GameFuseSettings asset:
   - Go to Assets > Create > GameFuse > Settings
   - Fill in your Game ID and Game API Key from the GameFuse dashboard

## Components

### GameFuseSampleManager

The main controller that coordinates between the different UI components. It handles initialization and manages the state of the UI based on authentication status.

### GameFuseAuthController

Handles user authentication interactions:
- Sign In
- Sign Up
- Forgot Password

### GameFuseDataController

Displays and refreshes user data:
- User profile information
- Store items
- Friends list
- Leaderboard

## UI Structure

The UI is built using UI Toolkit, with UXML for layout and USS for styling. The main components are:

- **GameFuseAuthPanel.uxml**: Contains the authentication UI panels.
- **GameFuseDataPanel.uxml**: Contains the data display panels.
- **GameFuseStyles.uss**: Contains styling for all UI elements.

## Customizing the Sample

You can customize the sample UI by:

1. Modifying the UXML files to change the layout
2. Editing the USS file to change the styling
3. Extending the controller scripts to add more functionality

## Additional Notes

- The sample uses UI Toolkit, which requires Unity 2021 LTS or later for optimal performance.
- For production use, you may want to enhance error handling and add loading indicators.
- This sample is designed to be a starting point; feel free to adapt it to your game's specific needs.

## Further Documentation

For more information on using the GameFuse SDK, see the [official documentation](https://docs.gamefuse.co).