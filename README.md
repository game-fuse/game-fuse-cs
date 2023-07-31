# Game Fuse C#
Use Game Fuse C# in your Unity project to easily add authenticaion, user data, leaderboards, in game stores and more all without writing and hosting any servers, or writing any API code. Its never been easier to store your data online! Game Fuse is always free for hobby and small indie projects. With larger usage there are metered usage fees.

## Getting Started
The first step of integrating GameFuse with your project, is to make an account at https://www.gamefuse.co.
After creating your account, add your first game and note the ID and API Token.
With this setup, you can now connect via your game client. Download and unzip the code from https://github.com/game-fuse/game-fuse-cs
Add this to your Unity project in the Scripts folder.

At this point in time, you would add the prefab in this folder GameFuseInitializer
You can also build this manually by adding new GameObject to your first scene, and add a number of script componenets:
- GameFuse.cs
- GameFuseLeaderboardEntry.cs
- GameFuseUser.cs
- GameFuseUtilities.cs
- GameFuseStoreItem.cs

At this point in time you have the library installed in your game, and you are ready to connect


## Connecting to Game Fuse

The first step in using GameFuse after it is installed and your account is regestered is to run the SetUpGame function. After this step you can run other functions to register users, sign in users, read and write game data.

In any script on your first scene you can run:

```

void Start () {
    var gameID = 'Your Game ID Here';
    var gameToken 'your Game Token Here';

    # 3rd param is the function below, GameFuse will call this function when it is done connecting your game
    GameFuse.SetUpGame(gameID, gameToken, GameSetUpCallback);
}



void GameSetUpCallback(string message, bool hasError) {
    if (hasError)
    {
        Debug.Error("Error connecting game: "+message);
    }
    else
    {
        Debug.Log("Game Connected Successfully")
        foreach (CloudLoginStoreItem storeItem in CloudLogin.GetStoreItems())
        {
            Debug.Log(storeItem.GetName() + ": " + storeItem.GetCost());
        }
    }
}

```

## Signing game users up

## Signing game users in

## Creating store items on the web

## Using the store in your game

## Using Credits

## Custom user data

## In game leaderboard

## Class Methods