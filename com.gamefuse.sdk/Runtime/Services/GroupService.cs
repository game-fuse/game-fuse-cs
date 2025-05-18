using GameFuse.Exceptions;
using GameFuse.Models;
using GameFuse.Transport;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GameFuse.Services
{
    /// <summary>
    /// Service for group-related operations.
    /// </summary>
    public class GroupService
    {
        private readonly ITransport _transport;
        
        /// <summary>
        /// Creates a new instance of the GroupService.
        /// </summary>
        /// <param name="transport">The transport to use for API requests.</param>
        public GroupService(ITransport transport)
        {
            _transport = transport ?? throw new ArgumentNullException(nameof(transport));
        }

        /// <summary>
        /// Creates a new group.
        /// </summary>
        /// <param name="userId">The ID of the user creating the group.</param>
        /// <param name="name">The name of the group.</param>
        /// <param name="groupType">The type of group (e.g., "Public", "Private").</param>
        /// <param name="canAutoJoin">Whether users can automatically join the group.</param>
        /// <param name="isInviteOnly">Whether the group is invite-only.</param>
        /// <param name="maxGroupSize">The maximum number of members allowed in the group.</param>
        /// <param name="searchable">Whether the group can be found in searches.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The created group.</returns>
        public Task<Group> CreateGroupAsync(int userId, string name, string groupType, bool canAutoJoin, bool isInviteOnly, int maxGroupSize, bool searchable, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrEmpty(groupType)) throw new ArgumentNullException(nameof(groupType));
            if (maxGroupSize <= 0) throw new ArgumentOutOfRangeException(nameof(maxGroupSize), "Max group size must be positive.");
            
            var request = new Dictionary<string, object>
            {
                ["creator_id"] = userId,
                ["name"] = name,
                ["group_type"] = groupType,
                ["can_auto_join"] = canAutoJoin,
                ["is_invite_only"] = isInviteOnly,
                ["max_group_size"] = maxGroupSize,
                ["searchable"] = searchable
            };
            
            return _transport.PostAsync<Dictionary<string, object>, Group>("groups", request, null, cancellationToken);
        }

        /// <summary>
        /// Gets a group by ID.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The group.</returns>
        public Task<Group> GetGroupAsync(int groupId, CancellationToken cancellationToken = default)
        {
            if (groupId <= 0) throw new ArgumentOutOfRangeException(nameof(groupId), "Group ID must be positive.");
            
            return _transport.GetAsync<Group>($"groups/{groupId}", null, cancellationToken);
        }

        /// <summary>
        /// Gets all groups for a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of groups.</returns>
        public async Task<IReadOnlyList<Group>> GetUserGroupsAsync(int userId, CancellationToken cancellationToken = default)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            
            var response = await _transport.GetAsync<List<Group>>($"users/{userId}/groups", null, cancellationToken);
            return response.AsReadOnly();
        }

        /// <summary>
        /// Updates a group.
        /// </summary>
        /// <param name="groupId">The ID of the group to update.</param>
        /// <param name="name">The new name of the group.</param>
        /// <param name="groupType">The new type of group.</param>
        /// <param name="canAutoJoin">Whether users can automatically join the group.</param>
        /// <param name="isInviteOnly">Whether the group is invite-only.</param>
        /// <param name="maxGroupSize">The maximum number of members allowed in the group.</param>
        /// <param name="searchable">Whether the group can be found in searches.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The updated group.</returns>
        public Task<Group> UpdateGroupAsync(int groupId, string name = null, string groupType = null, bool? canAutoJoin = null, bool? isInviteOnly = null, int? maxGroupSize = null, bool? searchable = null, CancellationToken cancellationToken = default)
        {
            if (groupId <= 0) throw new ArgumentOutOfRangeException(nameof(groupId), "Group ID must be positive.");
            
            var request = new Dictionary<string, object>();
            
            if (!string.IsNullOrEmpty(name))
            {
                request["name"] = name;
            }
            
            if (!string.IsNullOrEmpty(groupType))
            {
                request["group_type"] = groupType;
            }
            
            if (canAutoJoin.HasValue)
            {
                request["can_auto_join"] = canAutoJoin.Value;
            }
            
            if (isInviteOnly.HasValue)
            {
                request["is_invite_only"] = isInviteOnly.Value;
            }
            
            if (maxGroupSize.HasValue)
            {
                if (maxGroupSize.Value <= 0) throw new ArgumentOutOfRangeException(nameof(maxGroupSize), "Max group size must be positive.");
                request["max_group_size"] = maxGroupSize.Value;
            }
            
            if (searchable.HasValue)
            {
                request["searchable"] = searchable.Value;
            }
            
            if (request.Count == 0)
            {
                throw new ArgumentException("At least one parameter must be provided to update.");
            }
            
            return _transport.PutAsync<Dictionary<string, object>, Group>($"groups/{groupId}", request, null, cancellationToken);
        }

        /// <summary>
        /// Deletes a group.
        /// </summary>
        /// <param name="groupId">The ID of the group to delete.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task DeleteGroupAsync(int groupId, CancellationToken cancellationToken = default)
        {
            if (groupId <= 0) throw new ArgumentOutOfRangeException(nameof(groupId), "Group ID must be positive.");
            
            return _transport.DeleteAsync($"groups/{groupId}", null, cancellationToken);
        }

        /// <summary>
        /// Adds a user to a group.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="userId">The ID of the user to add.</param>
        /// <param name="isAdmin">Whether the user should be an admin.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task AddUserToGroupAsync(int groupId, int userId, bool isAdmin = false, CancellationToken cancellationToken = default)
        {
            if (groupId <= 0) throw new ArgumentOutOfRangeException(nameof(groupId), "Group ID must be positive.");
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            
            var request = new Dictionary<string, object>
            {
                ["user_id"] = userId,
                ["is_admin"] = isAdmin
            };
            
            return _transport.PostAsync<Dictionary<string, object>>($"groups/{groupId}/members", request, null, cancellationToken);
        }

        /// <summary>
        /// Removes a user from a group.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="userId">The ID of the user to remove.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task RemoveUserFromGroupAsync(int groupId, int userId, CancellationToken cancellationToken = default)
        {
            if (groupId <= 0) throw new ArgumentOutOfRangeException(nameof(groupId), "Group ID must be positive.");
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            
            return _transport.DeleteAsync($"groups/{groupId}/members/{userId}", null, cancellationToken);
        }

        /// <summary>
        /// Updates a member's status in a group.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="userId">The ID of the user to update.</param>
        /// <param name="isAdmin">Whether the user should be an admin.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task UpdateGroupMemberAsync(int groupId, int userId, bool isAdmin, CancellationToken cancellationToken = default)
        {
            if (groupId <= 0) throw new ArgumentOutOfRangeException(nameof(groupId), "Group ID must be positive.");
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            
            var request = new Dictionary<string, object>
            {
                ["is_admin"] = isAdmin
            };
            
            return _transport.PutAsync<Dictionary<string, object>>($"groups/{groupId}/members/{userId}", request, null, cancellationToken);
        }

        /// <summary>
        /// Sends a request to join a group.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="userId">The ID of the user sending the request.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task SendJoinRequestAsync(int groupId, int userId, CancellationToken cancellationToken = default)
        {
            if (groupId <= 0) throw new ArgumentOutOfRangeException(nameof(groupId), "Group ID must be positive.");
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            
            var request = new Dictionary<string, int>
            {
                ["user_id"] = userId
            };
            
            return _transport.PostAsync<Dictionary<string, int>>($"groups/{groupId}/join_requests", request, null, cancellationToken);
        }

        /// <summary>
        /// Accepts a request to join a group.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="requestId">The ID of the join request.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task AcceptJoinRequestAsync(int groupId, int requestId, CancellationToken cancellationToken = default)
        {
            if (groupId <= 0) throw new ArgumentOutOfRangeException(nameof(groupId), "Group ID must be positive.");
            if (requestId <= 0) throw new ArgumentOutOfRangeException(nameof(requestId), "Request ID must be positive.");
            
            return _transport.PutAsync<Dictionary<string, object>>($"groups/{groupId}/join_requests/{requestId}/accept", new Dictionary<string, object>(), null, cancellationToken);
        }

        /// <summary>
        /// Rejects a request to join a group.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="requestId">The ID of the join request.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task RejectJoinRequestAsync(int groupId, int requestId, CancellationToken cancellationToken = default)
        {
            if (groupId <= 0) throw new ArgumentOutOfRangeException(nameof(groupId), "Group ID must be positive.");
            if (requestId <= 0) throw new ArgumentOutOfRangeException(nameof(requestId), "Request ID must be positive.");
            
            return _transport.DeleteAsync($"groups/{groupId}/join_requests/{requestId}", null, cancellationToken);
        }

        /// <summary>
        /// Invites a user to join a group.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="userId">The ID of the user sending the invite.</param>
        /// <param name="inviteeId">The ID of the user to invite.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task InviteUserToGroupAsync(int groupId, int userId, int inviteeId, CancellationToken cancellationToken = default)
        {
            if (groupId <= 0) throw new ArgumentOutOfRangeException(nameof(groupId), "Group ID must be positive.");
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            if (inviteeId <= 0) throw new ArgumentOutOfRangeException(nameof(inviteeId), "Invitee ID must be positive.");
            
            var request = new Dictionary<string, int>
            {
                ["inviter_id"] = userId,
                ["invitee_id"] = inviteeId
            };
            
            return _transport.PostAsync<Dictionary<string, int>>($"groups/{groupId}/invites", request, null, cancellationToken);
        }

        /// <summary>
        /// Accepts an invitation to join a group.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="inviteId">The ID of the invite.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task AcceptGroupInviteAsync(int groupId, int inviteId, CancellationToken cancellationToken = default)
        {
            if (groupId <= 0) throw new ArgumentOutOfRangeException(nameof(groupId), "Group ID must be positive.");
            if (inviteId <= 0) throw new ArgumentOutOfRangeException(nameof(inviteId), "Invite ID must be positive.");
            
            return _transport.PutAsync<Dictionary<string, object>>($"groups/{groupId}/invites/{inviteId}/accept", new Dictionary<string, object>(), null, cancellationToken);
        }

        /// <summary>
        /// Rejects an invitation to join a group.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="inviteId">The ID of the invite.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task RejectGroupInviteAsync(int groupId, int inviteId, CancellationToken cancellationToken = default)
        {
            if (groupId <= 0) throw new ArgumentOutOfRangeException(nameof(groupId), "Group ID must be positive.");
            if (inviteId <= 0) throw new ArgumentOutOfRangeException(nameof(inviteId), "Invite ID must be positive.");
            
            return _transport.DeleteAsync($"groups/{groupId}/invites/{inviteId}", null, cancellationToken);
        }

        /// <summary>
        /// Searches for groups by name.
        /// </summary>
        /// <param name="query">The search query.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of groups matching the search criteria.</returns>
        public async Task<IReadOnlyList<Group>> SearchGroupsAsync(string query, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(query)) throw new ArgumentNullException(nameof(query));
            
            var response = await _transport.GetAsync<List<Group>>($"groups/search?query={Uri.EscapeDataString(query)}", null, cancellationToken);
            return response.AsReadOnly();
        }
    }
}