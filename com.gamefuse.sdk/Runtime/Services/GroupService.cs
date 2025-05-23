using GameFuse.Exceptions;
using GameFuse.Models.Shared;
using GameFuse.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Creates a new group for the authenticated user.
        /// </summary>
        /// <param name="payload">The details of the group to create.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The newly created Group object.</returns>
        public async Task<Group> CreateGroupAsync(CreateGroupPayload payload, CancellationToken cancellationToken = default)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));
            if (string.IsNullOrEmpty(payload.Name)) throw new ArgumentException("Group name cannot be empty.", nameof(payload.Name));

            // API Path: POST /api/v3/groups
            // The ITransport instance used by this service (via GameFuseUser) should be
            // configured with the user's authentication-token.
            // Expected HTTP status 201 Created. Transport layer should handle.
            var createdGroup = await _transport.PostAsync<CreateGroupPayload, Group>("groups", payload, null, cancellationToken);

            // Initialize lists if API returns null for them (good practice)
            if (createdGroup != null)
            {
                createdGroup.Members ??= new System.Collections.Generic.List<Friend>();
                createdGroup.Admins ??= new System.Collections.Generic.List<Friend>();
                createdGroup.JoinRequests ??= new System.Collections.Generic.List<GroupConnectionResponse>();
                createdGroup.Invites ??= new System.Collections.Generic.List<GroupConnectionResponse>();
            }
            

            return createdGroup;
        }

        /// <summary>
        /// Retrieves a list of available groups on the platform (summary view).
        /// Requires user authentication.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A response object containing a list of group summaries.</returns>
        public async Task<FetchAllGroupsResponse> FetchAllGroupsAsync(CancellationToken cancellationToken = default)
        {
            // API Path: GET /api/v3/groups
            // User authentication is required (implicit from the auth token).
            var response = await _transport.GetAsync<FetchAllGroupsResponse>("groups", null, cancellationToken);

            if (response == null) return new FetchAllGroupsResponse();
            response.Groups ??= new List<GroupSummary>();

            return response;
        }

        /// <summary>
        /// Retrieves the full details of a specific group.
        /// Requires user authentication.
        /// </summary>
        /// <param name="groupId">The ID of the group to fetch details for.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A Group object containing full details of the group.</returns>
        public async Task<Group> FetchGroupDetailsAsync(int groupId, CancellationToken cancellationToken = default)
        {
            if (groupId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(groupId), "Group ID must be positive.");
            }

            // API Path: GET /api/v3/groups/{id}
            // User authentication is required (implicit from the auth token).
            string path = $"groups/{groupId}";
            var groupDetails = await _transport.GetAsync<Group>(path, null, cancellationToken);

            // Initialize lists if API returns null for them
            if (groupDetails != null)
            {
                groupDetails.Members ??= new List<Friend>();
                groupDetails.Admins ??= new List<Friend>();
                groupDetails.JoinRequests ??= new List<GroupConnectionResponse>();
                groupDetails.Invites ??= new List<GroupConnectionResponse>();
            }
            // else: Handle cases where groupDetails might be null if API returns non-success
            // or if _transport.GetAsync can return null on certain conditions.
            // Usually, transport throws for HTTP errors.

            return groupDetails;
        }

        /// <summary>
        /// Creates a new group connection, which may involve sending an invite (admin action, not covered here yet)
        /// or processing a membership request (user action).
        /// The authenticated user is the one initiating this connection.
        /// </summary>
        /// <param name="groupId">The ID of the group to connect to.</param>
        /// <param name="requestingUserId">The ID of the user requesting to join (should be the authenticated user's ID).</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A GroupConnectionResponse object detailing the connection attempt.</returns>
        public async Task<GroupConnectionResponse> SendGroupConnectionRequestAsync(int groupId, int requestingUserId, CancellationToken cancellationToken = default)
        {
            if (groupId <= 0) throw new ArgumentOutOfRangeException(nameof(groupId), "Group ID must be positive.");
            if (requestingUserId <= 0) throw new ArgumentOutOfRangeException(nameof(requestingUserId), "Requesting User ID must be positive.");

            var payload = new SendGroupConnectionRequestPayload
            {
                GroupId = groupId,
                UserId = requestingUserId
            };

            // API Path: POST /api/v3/group_connections
            string path = "group_connections";
            var response = await _transport.PostAsync<SendGroupConnectionRequestPayload, GroupConnectionResponse>(path, payload, null, cancellationToken);

            if (response == null)
            {
                // Or throw specific exception
                return new GroupConnectionResponse { Status = "error_null_response" };
            }

            return response;
        }

        /// <summary>
        /// Manages a group membership request by updating its status (e.g., accept or decline).
        /// This action is typically performed by a group admin.
        /// </summary>
        /// <param name="groupConnectionId">The ID of the group_connection (join request) to manage.</param>
        /// <param name="newStatus">The new status to set (e.g., "accepted", "declined").</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A response indicating the updated ID and status of the group connection.</returns>
        public async Task<GroupConnectionStatusUpdateResponse> ManageGroupMembershipRequestAsync(int groupConnectionId, string newStatus, CancellationToken cancellationToken = default)
        {
            if (groupConnectionId <= 0) throw new ArgumentOutOfRangeException(nameof(groupConnectionId), "Group Connection ID must be positive.");
            if (string.IsNullOrEmpty(newStatus) || (newStatus.ToLower() != "accepted" && newStatus.ToLower() != "declined"))
            {
                throw new ArgumentException("New status must be 'accepted' or 'declined'.", nameof(newStatus));
            }

            var payload = new UpdateGroupConnectionStatusPayload
            {
                Status = newStatus.ToLower()
            };

            // API Path: PUT /api/v3/group_connections/{id}
            string path = $"group_connections/{groupConnectionId}";
            var response = await _transport.PutAsync<UpdateGroupConnectionStatusPayload, GroupConnectionStatusUpdateResponse>(path, payload, null, cancellationToken);

            if (response == null)
            {
                // Or throw specific exception
                return new GroupConnectionStatusUpdateResponse { Id = groupConnectionId, Status = "error_null_response" };
            }

            return response;
        }

        /// <summary>
        /// Adds new attributes to a group.
        /// </summary>
        /// <param name="groupId">The ID of the group to add attributes to.</param>
        /// <param name="payload">The payload containing a list of attributes to create.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A response containing the list of created attributes with their server-assigned IDs.</returns>
        public async Task<CreateGroupAttributesResponse> CreateGroupAttributesAsync(int groupId, CreateGroupAttributesPayload payload, CancellationToken cancellationToken = default)
        {
            if (groupId <= 0) throw new ArgumentOutOfRangeException(nameof(groupId), "Group ID must be positive.");
            if (payload == null) throw new ArgumentNullException(nameof(payload));
            if (payload.Attributes == null || !payload.Attributes.Any())
                throw new ArgumentException("Attributes list cannot be null or empty.", nameof(payload.Attributes));

            // API Path: POST /api/v3/groups/{id}/add_attribute
            string path = $"groups/{groupId}/add_attribute";

            // The payload to the API is an object with an "attributes" key, which is a list.
            var response = await _transport.PostAsync<CreateGroupAttributesPayload, CreateGroupAttributesResponse>(path, payload, null, cancellationToken);

            if (response == null)
            {
                return new CreateGroupAttributesResponse(); // Or throw, or return specific error indicator
            }
            response.Attributes ??= new List<GroupAttributeResponseItem>();

            return response;
        }

        /// <summary>
        /// Retrieves all attributes for a specific group.
        /// Requires user authentication (user must have permission to view attributes, typically being a member).
        /// </summary>
        /// <param name="groupId">The ID of the group to fetch attributes for.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A response object containing a list of the group's attributes.</returns>
        public async Task<FetchGroupAttributesResponse> FetchGroupAttributesAsync(int groupId, CancellationToken cancellationToken = default)
        {
            if (groupId <= 0) throw new ArgumentOutOfRangeException(nameof(groupId), "Group ID must be positive.");

            // API Path: GET /api/v3/groups/{id}/attributes
            string path = $"groups/{groupId}/attributes";
            var response = await _transport.GetAsync<FetchGroupAttributesResponse>(path, null, cancellationToken);

            if (response == null)
            {
                return new FetchGroupAttributesResponse(); // Or throw
            }
            response.Attributes ??= new List<GroupAttributeResponseItem>();

            return response;
        }

        /// <summary>
        /// Modifies an existing attribute of a group.
        /// The authenticated user must have permission (e.g., creator or admin).
        /// </summary>
        /// <param name="groupId">The ID of the group whose attribute is being modified.</param>
        /// <param name="payload">The payload containing the key of the attribute to modify and its new value.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The updated GroupAttributeResponseItem.</returns>
        public async Task<GroupAttributeResponseItem> ModifyGroupAttributeAsync(int groupId, ModifyGroupAttributePayload payload, CancellationToken cancellationToken = default)
        {
            if (groupId <= 0) throw new ArgumentOutOfRangeException(nameof(groupId), "Group ID must be positive.");
            if (payload == null) throw new ArgumentNullException(nameof(payload));
            if (string.IsNullOrEmpty(payload.Key)) throw new ArgumentException("Attribute key cannot be empty.", nameof(payload.Key));
            // Value can potentially be empty string, so no check for IsNullOrEmpty on payload.Value unless API forbids it.

            // API Path: PATCH /api/v3/groups/{id}/modify_attribute
            string path = $"groups/{groupId}/modify_attribute";

            // Your ITransport needs a PatchAsync method.
            // Assuming: public Task<TResponse> PatchAsync<TRequest, TResponse>(string path, TRequest body, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default)
            var modifiedAttribute = await _transport.PatchAsync<ModifyGroupAttributePayload, GroupAttributeResponseItem>(path, payload, null, cancellationToken);

            // It's possible the API returns a 200 OK with the updated attribute, or 204 No Content.
            // The example response in the doc shows the full attribute object, so we expect it.
            if (modifiedAttribute == null)
            {
                // Handle case where PATCH might return null or an error not caught by transport
                // For now, we'll assume transport throws or returns the deserialized object.
                // Or throw a specific exception e.g. GameFuseApiException("Failed to modify attribute or API returned no content.");
            }

            return modifiedAttribute;
        }

        /*
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
        }*/
    }
}