using GameFuse.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GameFuse
{
    public partial class GameFuseUser
    {
        /// <summary>
        /// Creates a new group.
        /// </summary>
        /// <param name="name">The name of the group.</param>
        /// <param name="groupType">The type of group (e.g., "Public", "Private").</param>
        /// <param name="canAutoJoin">Whether users can automatically join the group.</param>
        /// <param name="isInviteOnly">Whether the group is invite-only.</param>
        /// <param name="maxGroupSize">The maximum number of members allowed in the group.</param>
        /// <param name="searchable">Whether the group can be found in searches.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The created group.</returns>
        public Task<Group> CreateGroupAsync(string name, string groupType, bool canAutoJoin, bool isInviteOnly, int maxGroupSize, bool searchable, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _groupService.CreateGroupAsync(Id, name, groupType, canAutoJoin, isInviteOnly, maxGroupSize, searchable, cancellationToken);
        }

        /// <summary>
        /// Gets a group by ID.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The group.</returns>
        public Task<Group> GetGroupAsync(int groupId, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _groupService.GetGroupAsync(groupId, cancellationToken);
        }

        /// <summary>
        /// Gets all groups for the current user.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of groups.</returns>
        public Task<IReadOnlyList<Group>> GetUserGroupsAsync(CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _groupService.GetUserGroupsAsync(Id, cancellationToken);
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
            EnsureAuthenticated();
            return _groupService.UpdateGroupAsync(groupId, name, groupType, canAutoJoin, isInviteOnly, maxGroupSize, searchable, cancellationToken);
        }

        /// <summary>
        /// Deletes a group.
        /// </summary>
        /// <param name="groupId">The ID of the group to delete.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task DeleteGroupAsync(int groupId, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _groupService.DeleteGroupAsync(groupId, cancellationToken);
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
            EnsureAuthenticated();
            return _groupService.AddUserToGroupAsync(groupId, userId, isAdmin, cancellationToken);
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
            EnsureAuthenticated();
            return _groupService.RemoveUserFromGroupAsync(groupId, userId, cancellationToken);
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
            EnsureAuthenticated();
            return _groupService.UpdateGroupMemberAsync(groupId, userId, isAdmin, cancellationToken);
        }

        /// <summary>
        /// Sends a request to join a group.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task SendJoinRequestAsync(int groupId, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _groupService.SendJoinRequestAsync(groupId, Id, cancellationToken);
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
            EnsureAuthenticated();
            return _groupService.AcceptJoinRequestAsync(groupId, requestId, cancellationToken);
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
            EnsureAuthenticated();
            return _groupService.RejectJoinRequestAsync(groupId, requestId, cancellationToken);
        }

        /// <summary>
        /// Invites a user to join a group.
        /// </summary>
        /// <param name="groupId">The ID of the group.</param>
        /// <param name="inviteeId">The ID of the user to invite.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task InviteUserToGroupAsync(int groupId, int inviteeId, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _groupService.InviteUserToGroupAsync(groupId, Id, inviteeId, cancellationToken);
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
            EnsureAuthenticated();
            return _groupService.AcceptGroupInviteAsync(groupId, inviteId, cancellationToken);
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
            EnsureAuthenticated();
            return _groupService.RejectGroupInviteAsync(groupId, inviteId, cancellationToken);
        }

        /// <summary>
        /// Searches for groups by name.
        /// </summary>
        /// <param name="query">The search query.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A list of groups matching the search criteria.</returns>
        public Task<IReadOnlyList<Group>> SearchGroupsAsync(string query, CancellationToken cancellationToken = default)
        {
            EnsureAuthenticated();
            return _groupService.SearchGroupsAsync(query, cancellationToken);
        }
    }
}