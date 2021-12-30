﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bluong4_Job_Bank.Data;
using Bluong4_Job_Bank.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Bluong4_Job_Bank.Controllers
{
    [Authorize(Roles = "Security")]
    public class UserRolesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        string UserName;

        public UserRolesController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            UserName = _httpContextAccessor.HttpContext?.User.Identity.Name;
        }
        // GET: User
        public async Task<IActionResult> Index()
        {
            var users = await (from u in _context.Users
                               .OrderBy(u => u.UserName)
                               select new UserVM
                               {
                                   Id = u.Id,
                                   UserName = u.UserName
                               }).ToListAsync();
            foreach (var u in users)
            {
                var user = await _userManager.FindByIdAsync(u.Id);
                u.userRoles = await _userManager.GetRolesAsync(user);
            };
            return View(users);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }
            var _user = await _userManager.FindByIdAsync(id);//IdentityRole
            if (_user == null)
            {
                return NotFound();
            }
            UserVM user = new UserVM
            {
                Id = _user.Id,
                UserName = _user.UserName,
                userRoles = await _userManager.GetRolesAsync(_user)
            };
            PopulateAssignedRoleData(user);
            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string Id, string[] selectedRoles)
        {
            var _user = await _userManager.FindByIdAsync(Id);//IdentityRole
            UserVM user = new UserVM
            {
                Id = _user.Id,
                UserName = _user.UserName,
                userRoles = await _userManager.GetRolesAsync(_user)
            };
            try
            {
                if (_user.ToString() == UserName)
                {
                    ModelState.AddModelError(string.Empty,
                                    "You cannot modify your Role by yourself.");
                }
                else
                {
                    await UpdateUserRoles(selectedRoles, user);
                    return RedirectToAction("Index");
                }
                    
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty,
                                "Unable to save changes.");
            }
            PopulateAssignedRoleData(user);
            return View(user);
        }

        private void PopulateAssignedRoleData(UserVM user)
        {//Prepare checkboxes for all Roles
            var allRoles = _context.Roles;
            var currentRoles = user.userRoles;
            //var viewModel = new List<RoleVM>();
            var selected = new List<RoleVM>();
            var available = new List<RoleVM>();
            //foreach (var r in allRoles)
            //{
            //    viewModel.Add(new RoleVM
            //    {
            //        RoleId = r.Id,
            //        RoleName = r.Name,
            //        Assigned = currentRoles.Contains(r.Name)
            //    });
            //}

            foreach (var r in allRoles)
            {
                if(currentRoles.Contains(r.Name))
                {
                    selected.Add(new RoleVM
                    {
                        
                        RoleId = r.Id,
                        RoleName = r.Name,
                        Assigned = currentRoles.Contains(r.Name)
                    });
                }
                else
                {
                    available.Add(new RoleVM
                    {
                        
                        RoleId = r.Id,
                        RoleName = r.Name,
                        Assigned = currentRoles.Contains(r.Name)
                    });
                }
                
            }

            //ViewBag.Roles = viewModel;
            //ViewBag.Selected = selected;
            //ViewBag.Available = available;
            ViewData["selOpts"] = new MultiSelectList(selected.OrderBy(r => r.RoleName), "RoleName", "RoleName");
            ViewData["availOpts"] = new MultiSelectList(available.OrderBy(r => r.RoleName), "RoleName", "RoleName");
        }

        private async Task UpdateUserRoles(string[] selectedRoles, UserVM userToUpdate)
        {
            var userRoles = userToUpdate.userRoles;//Current roles use is in
            var _user = await _userManager.FindByIdAsync(userToUpdate.Id);//IdentityUser

            if (selectedRoles == null)
            {
                //No roles selected so just remove any currently assigned
                foreach (var r in userRoles)
                {
                    await _userManager.RemoveFromRoleAsync(_user, r);
                }
            }
            else
            {
                //At least one role checked so loop through all the roles
                //and add or remove as required

                //We need to do this next line because foreach loops don't always work well
                //for data returned by EF when working async.  Pulling it into an IList<>
                //first means we can safely loop over the colleciton making async calls and avoid
                //the error 'New transaction is not allowed because there are other threads running in the session'
                IList<IdentityRole> allRoles = _context.Roles.ToList<IdentityRole>();

                foreach (var r in allRoles)
                {
                    if (selectedRoles.Contains(r.Name))
                    {
                        if (!userRoles.Contains(r.Name))
                        {
                            await _userManager.AddToRoleAsync(_user, r.Name);
                        }
                    }
                    else
                    {
                        if (userRoles.Contains(r.Name))
                        {
                            await _userManager.RemoveFromRoleAsync(_user, r.Name);
                        }
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
                _userManager.Dispose();
            }
            base.Dispose(disposing);
        }



    }
}
