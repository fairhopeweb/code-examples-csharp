﻿// <copyright file="SMSDelivery.cs" company="DocuSign">
// Copyright (c) DocuSign. All rights reserved.
// </copyright>

namespace DocuSign.CodeExamples.ESignature.Controllers
{
    using System;
    using DocuSign.CodeExamples.Common;
    using DocuSign.CodeExamples.Controllers;
    using DocuSign.CodeExamples.Models;
    using Microsoft.AspNetCore.Mvc;

    [Area("eSignature")]
    [Route("Eg037")]
    public class SMSDelivery : EgController
    {
        public SMSDelivery(DSConfiguration config, LauncherTexts launcherTexts, IRequestItemsService requestItemsService)
            : base(config, launcherTexts, requestItemsService)
        {
            this.CodeExampleText = this.GetExampleText(EgName, ExamplesAPIType.ESignature);
            this.ViewBag.title = this.CodeExampleText.ExampleName;
        }

        public override string EgName => "Eg037";

        [HttpPost]
        [SetViewBag]
        public IActionResult Create(string signerName, string signerCountryCode, string signerPhoneNumber, string ccName, string ccCountryCode, string ccPhoneNumber)
        {
            // Check the token with minimal buffer time.
            bool tokenOk = this.CheckToken(3);
            if (!tokenOk)
            {
                // We could store the parameters of the requested operation
                // so it could be restarted automatically.
                // But since it should be rare to have a token issue here,
                // we'll make the user re-enter the form data after
                // authentication.
                this.RequestItemsService.EgName = this.EgName;
                return this.Redirect("/ds/mustAuthenticate");
            }

            var basePath = this.RequestItemsService.Session.BasePath + "/restapi";
            var accessToken = this.RequestItemsService.User.AccessToken; // Represents your {ACCESS_TOKEN}
            var accountId = this.RequestItemsService.Session.AccountId; // Represents your {ACCOUNT_ID}

            // Call the Examples API method to create and send an envelope and notify recipients via SMS
            var envelopeId = global::ESignature.Examples.SMSDelivery.SendRequestViaSMS(
                accessToken,
                basePath,
                accountId,
                signerName,
                signerCountryCode,
                signerPhoneNumber,
                ccName,
                ccCountryCode,
                ccPhoneNumber,
                this.Config.DocDocx,
                this.Config.DocPdf,
                this.RequestItemsService.Status);

            this.RequestItemsService.EnvelopeId = envelopeId;

            this.ViewBag.h1 = this.CodeExampleText.ExampleName;
            this.ViewBag.message = string.Format(this.CodeExampleText.ResultsPageText, envelopeId);
            return this.View("example_done");
        }
    }
}