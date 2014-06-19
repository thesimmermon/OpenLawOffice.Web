﻿// -----------------------------------------------------------------------
// <copyright file="AccountController.cs" company="Nodine Legal, LLC">
// Licensed to Nodine Legal, LLC under one
// or more contributor license agreements.  See the NOTICE file
// distributed with this work for additional information
// regarding copyright ownership.  Nodine Legal, LLC licenses this file
// to you under the Apache License, Version 2.0 (the
// "License"); you may not use this file except in compliance
// with the License.  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.
// </copyright>
// -----------------------------------------------------------------------

namespace OpenLawOffice.WebClient.Controllers
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Collections.Generic;
    using System.Web.Security;
    using System;

    [HandleError(View = "Errors/Index", Order = 10)]
    public class AccountController : BaseController
    {
        public IFormsAuthenticationService FormsService { get; set; }
        public AccountMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        public ActionResult Login()
        {
            List<Common.Models.Account.Users> users = null;

            try
            {
                users = Data.Account.Users.List();
            }
            catch
            {
                return RedirectToAction("Index", "Installation");
            }

            if (users == null || users.Count < 1)
                return RedirectToAction("Register", "Account");

            return View();
        }

        [HttpPost]
        public ActionResult Login(ViewModels.Account.LoginViewModel viewModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ValidateUser(viewModel.Username, viewModel.Password))
                {
                    FormsService.SignIn(viewModel.Username, viewModel.RememberMe);
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(viewModel);
        }

        public ActionResult LogOff()
        {
            FormsService.SignOut();

            return RedirectToAction("Login", "Account");
        }

        public ActionResult Register()
        {
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View();
        }

        [HttpPost]
        public ActionResult Register(ViewModels.Account.RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus = MembershipService.CreateUser(model.UserName, model.Password, model.Email);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsService.SignIn(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", ViewModels.AccountValidation.ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View(model);
        }

        [Authorize(Roles="Administrators, Users, Clients")]
        public ActionResult ChangePassword(string currentPassword)
        {
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            if (!string.IsNullOrEmpty(currentPassword))
                return View(new ViewModels.Account.ChangePasswordViewModel()
                {
                    OldPassword = currentPassword
                });
            else
                return View();
        }

        [HttpPost]
        [Authorize(Roles = "Login, User")]
        public ActionResult ChangePassword(ViewModels.Account.ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View(model);
        }

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult Profile()
        {
            return View(new ViewModels.Account.ProfileViewModel() { Email = Membership.GetUser().Email });
        }

        [HttpPost]
        [Authorize(Roles = "Login, User")]
        public ActionResult Profile(ViewModels.Account.ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                MembershipUser user = Membership.GetUser();
                user.Email = model.Email;
                try
                {
                    Membership.UpdateUser(user);
                }
                catch (System.Configuration.Provider.ProviderException ex)
                {
                    if (ex.Message == "Duplicate E-mail address. The E-mail supplied is invalid.")
                        return RedirectToAction("Index", "Home");
                }
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Recovery(string reset, string username)
        {
            if ((reset != null) && (username != null))
            {
                MembershipUser currentUser = Membership.GetUser(username);
                if (HashResetParams(currentUser.UserName, currentUser.ProviderUserKey.ToString()) == reset)
                {
                    string tempPw = currentUser.ResetPassword();
                    FormsService.SignIn(currentUser.UserName, true);
                    return RedirectToAction("ChangePassword", new { currentPassword = tempPw });
                }
            }

            return View();
        }

        public static string HashResetParams(string username, string guid)
        {
            byte[] bytesofLink = System.Text.Encoding.UTF8.GetBytes(username + guid);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            string HashParams = BitConverter.ToString(md5.ComputeHash(bytesofLink)).Replace("-", "").ToLower();

            return HashParams;
        }

        public void SendResetEmail(System.Web.Security.MembershipUser user)
        {
            string site = Common.Settings.Manager.Instance.System.WebsiteUrl.ToString();

            if (!site.EndsWith("\\") && !site.EndsWith("/"))
                site += "/";

            EmailView<ViewModels.Account.RecoveryViewModel>("~/Views/Templates/AccountRecoveryEmail.aspx",
                new ViewModels.Account.RecoveryViewModel()
                {
                    Email = user.Email,
                    ResetPwAddress = site + "Account/Recovery/?username=" + user.UserName + "&reset=" + HashResetParams(user.UserName, user.ProviderUserKey.ToString()),
                    IpAddress = Common.Utilities.GetClientIpAddress(Request),
                    UserName = user.UserName
                }, user.Email, "Account Recovery");
        }

        [HttpPost]
        public ActionResult Recovery(ViewModels.Account.RecoveryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(viewModel.UserName))
                {
                    MembershipUser user = Membership.GetUser(viewModel.UserName);
                    if (user == null)
                        ViewData["Error"] = "username";
                    else
                    {
                        SendResetEmail(user);
                        return View("RecoveryEmailSent");
                    }
                }
                else
                {
                    MembershipUserCollection coll = Membership.FindUsersByEmail(viewModel.Email);
                    if (coll.Count != 1)
                    {
                        ViewData["Error"] = "email";
                    }
                    else
                    {
                        foreach (MembershipUser user in coll)
                        {
                            SendResetEmail(user);
                            return View("RecoveryEmailSent");
                        }
                    }
                }
            }

            return View();
        }
    }
}