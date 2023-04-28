// <copyright file="SharedAccess.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;

namespace ESignature.Examples
{
    public static class SharedAccess
    {
        public static NewUsersSummary ShareAccess(
            string accessToken,
            string basePath,
            string accountId,
            string activationCode,
            string agentEmail,
            string agentName)
        {
            // Step 1 start
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var usersApi = new UsersApi(docuSignClient);

            // Step 1 end

            // Step 2 start
            var newUser = new NewUsersDefinition
            {
                NewUsers = new List<UserInformation>
                {
                    new UserInformation(activationCode, Email: agentEmail, UserName: agentName)
                }
            };

            var userSummary = usersApi.Create(accountId, newUser);

            // Step 2 end

            return userSummary;
        }

        public static EnvelopesInformation GetEnvelopesListStatus(
            string accessToken,
            string basePath,
            string accountId)
        {
            // Step 1 start
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var envelopesApi = new EnvelopesApi(docuSignClient);

            // Step 1 end

            // Step 2 start
            var date = DateTime.UtcNow.AddDays(-10).ToString("yyyy-MM-ddTHH:mmZ");
            var option = new EnvelopesApi.ListStatusChangesOptions()
            {
                fromDate = date
            };

            var envelopes = envelopesApi.ListStatusChanges(accountId, option);

            // Step 2 end
            return envelopes;
        }


        public static void CreateUserAuthorization(
            string accessToken,
            string basePath,
            string accountId,
            string impersonatedUserId,
            string agentUserId)
        {
            var docuSignClient = new DocuSignClient(basePath);
            docuSignClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);
            var accountApi = new AccountsApi(docuSignClient);

            var authRequest = new UserAuthorizationCreateRequest(
                Permission: "manage",
                AgentUser: new AuthorizationUser(AccountId: accountId, UserId: agentUserId));

            accountApi.CreateUserAuthorization(accountId, impersonatedUserId, authRequest);
        }
    }
}