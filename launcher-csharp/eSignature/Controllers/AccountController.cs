﻿// <copyright file="AccountController.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.Controllers
{
    using System;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Models;
    using DocuSign.eSign.Client;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [Route("ds/[action]")]
    public class AccountController : Controller
    {
        private IRequestItemsService requestItemsService;

        private IConfiguration Configuration { get; }

        private LauncherTexts LauncherTexts { get; }

        public AccountController(IRequestItemsService requestItemsService, LauncherTexts launcherTexts, IConfiguration configuration)
        {
            this.requestItemsService = requestItemsService;
            this.Configuration = configuration;
            this.LauncherTexts = launcherTexts;
        }

        [HttpGet]
        public IActionResult Login(string authType = "CodeGrant", string returnUrl = "/")
        {
            if (authType == "CodeGrant")
            {
                returnUrl += "?egName=" + this.requestItemsService.EgName;
                return this.Challenge(new AuthenticationProperties() { RedirectUri = returnUrl });
            }

            try
            {
                this.requestItemsService.UpdateUserFromJWT();
            }
            catch (ApiException apiExp)
            {
                // Consent for impersonation must be obtained to use JWT Grant
                if (apiExp.Message.Contains("consent_required"))
                {
                    return this.Redirect(this.BuildConsentURL());
                }
            }

            return this.LocalRedirect(returnUrl);
        }

        public IActionResult MustAuthenticate()
        {
            if (this.Configuration["ExamplesAPI"] == "Monitor")
            {
                // Monitor API supports JWT only
                return this.Login("JWT");
            }

            if (this.Configuration["quickstart"] == "true")
            {
                return this.Login();
            }

            this.ViewBag.SupportingTexts = this.LauncherTexts.ManifestStructure.SupportingTexts;
            return this.View();
        }

        public async System.Threading.Tasks.Task<IActionResult> Logout()
        {
            await AuthenticationHttpContextExtensions.SignOutAsync(this.HttpContext);
            this.requestItemsService.Logout();
            this.Configuration["quickstart"] = "false";
            return this.LocalRedirect("/?egName=home");
        }

        /// <summary>
        /// Generates a URL that can be used to obtain consent needed for the JWT Flow
        /// </summary>
        /// <returns>Consent URL</returns>
        private string BuildConsentURL()
        {
            // ESignature scopes
            var scopes = "signature impersonation";
            var apiType = Enum.Parse<ExamplesAPIType>(this.Configuration["ExamplesAPI"]);

            // Rooms scopes
            scopes += " dtr.rooms.read dtr.rooms.write dtr.documents.read dtr.documents.write "
                + "dtr.profile.read dtr.profile.write dtr.company.read dtr.company.write room_forms";
            
            // Click scopes
            scopes += " click.manage click.send";

            // Admin scopes
            scopes += " user_read user_write organization_read account_read group_read permission_read identity_provider_read domain_read";

            return this.Configuration["DocuSign:AuthorizationEndpoint"] + "?response_type=code" +
                "&scope=" + scopes +
                "&client_id=" + this.Configuration["DocuSignJWT:ClientId"] +
                "&redirect_uri=" + this.Configuration["DocuSign:AppUrl"] + "/ds/login?authType=JWT";
        }
    }
}
