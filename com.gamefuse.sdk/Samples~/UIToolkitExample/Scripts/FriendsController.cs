using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using GameFuseCSharp;

namespace GameFuse.UIToolkit
{
    public class FriendsController : BaseGameFuseUIController
    {
        // Friend UI elements
        private TextField friendUsernameField;
        private TextField friendshipIdField;
        private TextField friendUserIdField;
        private Button getAllFriendDataButton;
        private Button sendFriendRequestButton;
        private Button acceptFriendRequestButton;
        private Button declineFriendRequestButton;
        private Button cancelFriendRequestButton;
        private Button unfriendButton;
        private ScrollView friendsScrollView;
        private ScrollView incomingRequestsScrollView;
        private ScrollView outgoingRequestsScrollView;

        protected override void InitializeUI()
        {
            var content = rootElement.Q<VisualElement>("content-friends");
            
            // Fields
            friendUsernameField = content.Q<TextField>("friend-username");
            friendshipIdField = content.Q<TextField>("friendship-id");
            friendUserIdField = content.Q<TextField>("friend-user-id");
            
            // Buttons
            getAllFriendDataButton = content.Q<Button>("get-all-friend-data-button");
            sendFriendRequestButton = content.Q<Button>("send-friend-request-button");
            acceptFriendRequestButton = content.Q<Button>("accept-friend-request-button");
            declineFriendRequestButton = content.Q<Button>("decline-friend-request-button");
            cancelFriendRequestButton = content.Q<Button>("cancel-friend-request-button");
            unfriendButton = content.Q<Button>("unfriend-button");
            
            // ScrollViews
            friendsScrollView = content.Q<ScrollView>("friends-scroll");
            incomingRequestsScrollView = content.Q<ScrollView>("incoming-requests-scroll");
            outgoingRequestsScrollView = content.Q<ScrollView>("outgoing-requests-scroll");
        }

        protected override void RegisterCallbacks()
        {
            getAllFriendDataButton.RegisterCallback<ClickEvent>(async evt => await OnGetAllFriendDataClicked());
            sendFriendRequestButton.RegisterCallback<ClickEvent>(async evt => await OnSendFriendRequestClicked());
            acceptFriendRequestButton.RegisterCallback<ClickEvent>(async evt => await OnAcceptFriendRequestClicked());
            declineFriendRequestButton.RegisterCallback<ClickEvent>(async evt => await OnDeclineFriendRequestClicked());
            cancelFriendRequestButton.RegisterCallback<ClickEvent>(async evt => await OnCancelFriendRequestClicked());
            unfriendButton.RegisterCallback<ClickEvent>(async evt => await OnUnfriendClicked());
        }

        protected override void UnregisterCallbacks()
        {
            getAllFriendDataButton.UnregisterCallback<ClickEvent>(async evt => await OnGetAllFriendDataClicked());
            sendFriendRequestButton.UnregisterCallback<ClickEvent>(async evt => await OnSendFriendRequestClicked());
            acceptFriendRequestButton.UnregisterCallback<ClickEvent>(async evt => await OnAcceptFriendRequestClicked());
            declineFriendRequestButton.UnregisterCallback<ClickEvent>(async evt => await OnDeclineFriendRequestClicked());
            cancelFriendRequestButton.UnregisterCallback<ClickEvent>(async evt => await OnCancelFriendRequestClicked());
            unfriendButton.UnregisterCallback<ClickEvent>(async evt => await OnUnfriendClicked());
        }

        private async Task OnGetAllFriendDataClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to view friend data", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Get friend data in parallel
                var friendsTask = GameFuseUser.CurrentUser.GetFriendsAsync();
                var incomingRequestsTask = GameFuseUser.CurrentUser.GetIncomingFriendRequestsAsync();
                var outgoingRequestsTask = GameFuseUser.CurrentUser.GetOutgoingFriendRequestsAsync();
                
                await Task.WhenAll(friendsTask, incomingRequestsTask, outgoingRequestsTask);
                
                // Display friend data
                DisplayFriends(await friendsTask);
                DisplayIncomingFriendRequests(await incomingRequestsTask);
                DisplayOutgoingFriendRequests(await outgoingRequestsTask);
                
                LogMessage("Friend data retrieved successfully", LogType.Success);
            });
        }

        private async Task OnSendFriendRequestClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to send friend requests", LogType.Error);
                return;
            }
            
            string username = friendUsernameField.value;
            
            if (string.IsNullOrEmpty(username))
            {
                LogMessage("Username is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Create friend request data
                var request = new FriendRequestData
                {
                    Username = username
                };
                
                // Send friend request
                var response = await GameFuseUser.CurrentUser.SendFriendRequestAsync(request);
                
                // Clear username field
                friendUsernameField.value = string.Empty;
                
                // Refresh friend data
                await OnGetAllFriendDataClicked();
                
                LogMessage($"Friend request sent to {username}", LogType.Success);
            });
        }

        private async Task OnAcceptFriendRequestClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to accept friend requests", LogType.Error);
                return;
            }
            
            string friendshipId = friendshipIdField.value;
            
            if (string.IsNullOrEmpty(friendshipId))
            {
                LogMessage("Friendship ID is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Accept friend request
                var response = await GameFuseUser.CurrentUser.AcceptFriendRequestAsync(int.Parse(friendshipId));
                
                // Refresh friend data
                await OnGetAllFriendDataClicked();
                
                LogMessage("Friend request accepted", LogType.Success);
            });
        }

        private async Task OnDeclineFriendRequestClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to decline friend requests", LogType.Error);
                return;
            }
            
            string friendshipId = friendshipIdField.value;
            
            if (string.IsNullOrEmpty(friendshipId))
            {
                LogMessage("Friendship ID is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Decline friend request
                var response = await GameFuseUser.CurrentUser.DeclineFriendRequestAsync(int.Parse(friendshipId));
                
                // Refresh friend data
                await OnGetAllFriendDataClicked();
                
                LogMessage("Friend request declined", LogType.Success);
            });
        }

        private async Task OnCancelFriendRequestClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to cancel friend requests", LogType.Error);
                return;
            }
            
            string friendshipId = friendshipIdField.value;
            
            if (string.IsNullOrEmpty(friendshipId))
            {
                LogMessage("Friendship ID is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Cancel friend request
                var response = await GameFuseUser.CurrentUser.CancelFriendRequestAsync(int.Parse(friendshipId));
                
                // Refresh friend data
                await OnGetAllFriendDataClicked();
                
                LogMessage("Friend request cancelled", LogType.Success);
            });
        }

        private async Task OnUnfriendClicked()
        {
            if (GameFuseUser.CurrentUser == null)
            {
                LogMessage("You must be signed in to unfriend players", LogType.Error);
                return;
            }
            
            string userId = friendUserIdField.value;
            
            if (string.IsNullOrEmpty(userId))
            {
                LogMessage("User ID is required", LogType.Error);
                return;
            }
            
            await ExecuteAsync(async () =>
            {
                // Unfriend player
                var response = await GameFuseUser.CurrentUser.UnfriendPlayerAsync(int.Parse(userId));
                
                // Refresh friend data
                await OnGetAllFriendDataClicked();
                
                LogMessage("Player unfriended successfully", LogType.Success);
            });
        }

        private void DisplayFriends(UserInfo[] friends)
        {
            // Clear current display
            ClearScrollView(friendsScrollView);
            
            if (friends != null && friends.Length > 0)
            {
                friendsScrollView.Add(new Label($"You have {friends.Length} friends") { style = { unityFontStyleAndWeight = FontStyle.Bold } });
                
                for (int i = 0; i < friends.Length; i++)
                {
                    var friend = friends[i];
                    var properties = new Dictionary<string, string>
                    {
                        { "ID", friend.Id.ToString() },
                        { "Email", friend.Email },
                        { "Score", friend.Score.ToString() },
                        { "Credits", friend.Credits.ToString() }
                    };
                    
                    var friendItem = CreateListItem(friend.Username, properties);
                    friendsScrollView.Add(friendItem);
                }
            }
            else
            {
                friendsScrollView.Add(new Label("You don't have any friends yet"));
            }
        }

        private void DisplayIncomingFriendRequests(FriendRequest[] requests)
        {
            // Clear current display
            ClearScrollView(incomingRequestsScrollView);
            
            if (requests != null && requests.Length > 0)
            {
                incomingRequestsScrollView.Add(new Label($"You have {requests.Length} incoming requests") 
                { style = { unityFontStyleAndWeight = FontStyle.Bold } });
                
                for (int i = 0; i < requests.Length; i++)
                {
                    var request = requests[i];
                    var properties = new Dictionary<string, string>
                    {
                        { "ID", request.Id.ToString() },
                        { "Friendship ID", request.FriendshipId.ToString() },
                        { "Requested At", request.RequestedAt },
                    };
                    
                    var requestItem = CreateListItem(request.Username, properties);
                    incomingRequestsScrollView.Add(requestItem);
                }
            }
            else
            {
                incomingRequestsScrollView.Add(new Label("No incoming friend requests"));
            }
        }

        private void DisplayOutgoingFriendRequests(FriendRequest[] requests)
        {
            // Clear current display
            ClearScrollView(outgoingRequestsScrollView);
            
            if (requests != null && requests.Length > 0)
            {
                outgoingRequestsScrollView.Add(new Label($"You have {requests.Length} outgoing requests") 
                { style = { unityFontStyleAndWeight = FontStyle.Bold } });
                
                for (int i = 0; i < requests.Length; i++)
                {
                    var request = requests[i];
                    var properties = new Dictionary<string, string>
                    {
                        { "ID", request.Id.ToString() },
                        { "Friendship ID", request.FriendshipId.ToString() },
                        { "Requested At", request.RequestedAt },
                    };
                    
                    var requestItem = CreateListItem(request.Username, properties);
                    outgoingRequestsScrollView.Add(requestItem);
                }
            }
            else
            {
                outgoingRequestsScrollView.Add(new Label("No outgoing friend requests"));
            }
        }
    }
}