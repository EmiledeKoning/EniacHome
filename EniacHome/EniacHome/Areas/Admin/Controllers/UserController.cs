﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EniacHome.Areas.Admin.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using EniacHome.Models;
using System.Threading.Tasks;
using System.ComponentModel.Composition;

namespace EniacHome.Areas.Admin.Controllers
{
    public enum AuthorizationLevel
    {
        User,
        Moderator,
        Administrator,
    }
    
    [Authorize(Roles = "Administrator, Moderator")]
    public class UserController : Controller
    {
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Admin
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }


        public ActionResult List()
        {
            List<ApplicationUser> users = UserManager.Users.ToList();

            List<UsersListViewModel> model = users.Select(user => new UsersListViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                Address = user.Address,
                PostalCode = user.PostalCode,
                Country = user.Country,
                Role = GetHighestRole(UserManager.GetRoles(user.Id).ToArray())
            }).ToList();

            return View(model);
        }

        public async Task<ActionResult> Edit(string id)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(id);

            var roles = ApplicationDbContext.Create().Roles.Select(x => x.Name.ToString()).ToList();
            roles.Add("Disabled");
            UsersEditViewModel model = new UsersEditViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                FullName = user.FullName,
                Address = user.Address,
                PostalCode = user.PostalCode,
                Country = user.Country,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled,
                AccesFailedCount = user.AccessFailedCount,
                LockedOutEnabled = user.LockoutEnabled,
                LockedOutEndDateUtc = user.LockoutEndDateUtc ?? DateTime.Now.AddDays(-1),
                Roles = roles,
                CurrentRole = GetHighestRole(UserManager.GetRoles(user.Id).ToArray())

            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UsersEditViewModel model)
        {
            await UserManager.RemoveFromRoleAsync(model.Id, "Administrator");
            await UserManager.RemoveFromRoleAsync(model.Id, "Moderator");
            await UserManager.RemoveFromRoleAsync(model.Id, "User");
            ApplicationUser user = await UserManager.FindByIdAsync(model.Id);   

            switch (model.Roles[0].ToString())
            {
                case "Administrator":
                    if (HttpContext.GetOwinContext().Authentication.User.IsInRole("Administrator"))
                    {
                        await UserManager.AddToRoleAsync(model.Id, "User");
                        await UserManager.AddToRoleAsync(model.Id, "Moderator");
                        await UserManager.AddToRoleAsync(model.Id, "Administrator");
                        user.AccountConfirmed = true;
                        await UserManager.UpdateAsync(user);
                    }
                    else
                        ModelState.AddModelError("", "Warning: you are unauthorized.");
                    break;

                case "Moderator":
                    await UserManager.AddToRoleAsync(model.Id, "User");
                    await UserManager.AddToRoleAsync(model.Id, "Moderator");
                    user.AccountConfirmed = true;
                    await UserManager.UpdateAsync(user);
                    break;

                case "User":
                    await UserManager.AddToRolesAsync(model.Id, "User");
                    user.AccountConfirmed = true;
                    await UserManager.UpdateAsync(user);
                    break;
                default:
                    user.AccountConfirmed = false;
                    await UserManager.UpdateAsync(user);
                    break;           
            }
            return RedirectToAction("List");
        }

        public ActionResult Delete(string id, string ReturnUrl)
        {
            ApplicationUser user = UserManager.FindById(id);
            UserManager.Delete(user);
            return RedirectToLocal(ReturnUrl);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "User");
        }

        //
        // GET: /Account/ConfirmAccount
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmAccount(string userId)
        {
            if (userId == null)
            {
                return View("Error");
            }
            IdentityResult result = await UserManager.AddToRoleAsync(userId, "User");
            //send mail to user
            if (result.Succeeded)
            {
                ApplicationUser user = await UserManager.FindByIdAsync(userId);
                string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                string callbackUrl = Url.Action("ConfirmEmail", "Account", new { area = "", userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                await UserManager.SendEmailAsync(user.Id, "Confirm your Email", String.Format("Dear {0}, <br /><br />An administrator has approved your account. To start using our services, please confirm your email by clicking <a href=\"{1}\">Here</a>",
                    user.FullName,
                    callbackUrl
                    ));

                user.AccountConfirmed = true;
                result = await UserManager.UpdateAsync(user);
            }
            return View(result.Succeeded ? "ConfirmAccount" : "Error");
        }

        private string GetHighestRole(string[] Roles)
        {
            string highest = "";
            foreach (string role in Roles)
            {
                if (role.ToString() == AuthorizationLevel.Administrator.ToString())
                    return AuthorizationLevel.Administrator.ToString();
                if (role.ToString() == AuthorizationLevel.Moderator.ToString())
                    highest = AuthorizationLevel.Moderator.ToString();
                if (role.ToString() == AuthorizationLevel.User.ToString())
                {
                    if (highest != "Moderator")
                        highest = AuthorizationLevel.User.ToString();
                }


            }
            return highest;
        }
    }
}