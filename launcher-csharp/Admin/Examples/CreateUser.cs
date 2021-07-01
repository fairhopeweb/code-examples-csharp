﻿using DocuSign.OrgAdmin.Api;
using DocuSign.OrgAdmin.Client;
using DocuSign.OrgAdmin.Model;
using System;
using System.Collections.Generic;

namespace DocuSign.CodeExamples.Admin.Examples
{
    public class CreateUser
    {
        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="accessToken">Access Token for API call (OAuth)</param>
        /// <param name="basePath">BasePath for API calls (URI)</param>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="organizationId">The DocuSign organization ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="firstName">The first name of a new user</param>
        /// <param name="lastName">The last name of a new user</param>
        /// <param name="userName">The username of a new user</param>
        /// <param name="email">The email of a new user</param>
        /// <param name="permissionProfileId">The permission profile ID that will be used for a new user</param>
        /// <param name="groupId">The group ID that will be used for a new user</param>
        /// <returns>The response of creating a new user</returns>
        public static NewUserResponse CreateNewUser(
            string accessToken, string basePath, Guid accountId, Guid? organizationId, string firstName, string lastName,
            string userName, string email, long permissionProfileId, long groupId)
        {
            // Construct your API headers
            var apiClient = new ApiClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            var usersApi = new UsersApi(apiClient);
            NewUserRequest newUserRequest = ConstructNewUserRequest(permissionProfileId, groupId, accountId, email,
                firstName, lastName, userName);

            return usersApi.CreateUser(organizationId, newUserRequest);
        }

        /// <summary>
        /// Constructs a request for creating a new user
        /// </summary>
        /// <param name="accountId">The DocuSign Account ID (GUID or short version) for which the APIs call would be made</param>
        /// <param name="firstName">The first name of a new user</param>
        /// <param name="lastName">The last name of a new user</param>
        /// <param name="userName">The username of a new user</param>
        /// <param name="email">The email of a new user</param>
        /// <param name="permissionProfileId">The permission profile ID that will be used for a new user</param>
        /// <param name="groupId">The group ID that will be used for a new user</param>
        /// <returns>The request for creating a new user</returns>
        private static NewUserRequest ConstructNewUserRequest(
            long permissionProfileId, long groupId, Guid accountId, string email, string firstName, string lastName,
            string userName)
        {
            return new NewUserRequest
            {
                FirstName = firstName,
                LastName = lastName,
                UserName = userName,
                Email = email,
                Accounts = new List<NewUserRequestAccountProperties>
                {
                    new NewUserRequestAccountProperties
                    {
                        Id = accountId,
                        PermissionProfile = new PermissionProfileRequest
                        {
                            Id = permissionProfileId
                        },
                        Groups = new List<GroupRequest>
                        {
                            new GroupRequest
                            {
                                Id = groupId
                            }
                        }
                    }
                },
                AutoActivateMemberships = true
            };
        }
    }
}
