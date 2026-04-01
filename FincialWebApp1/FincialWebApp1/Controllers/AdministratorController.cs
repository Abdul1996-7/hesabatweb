using FincialWebApp1.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FincialWebApp1.Models;
using FincialWebApp1.ViewModels;
using System.Security.Claims;
using FincialWebApp1.Models;


namespace FincialWebApp1.Controllers
{
    [AllowAnonymous]
    //[Authorize(Roles = "Super Admin")]

    public class AdministratorController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly ILogger<AdministratorController> logger;
        private readonly FincialWebApp1Context _context;
       

        public AdministratorController(RoleManager<IdentityRole> roleManager,
                                        UserManager<IdentityUser> userManager,
                                        ILogger<AdministratorController> logger,
                                        FincialWebApp1Context context)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.logger = logger;
            _context = context;
        }
        [HttpGet]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            // UserManager service GetClaimsAsync method gets all the current claims of the user
            var existingUserClaims = await userManager.GetClaimsAsync(user);

            var model = new UserClaimsViewModel
            {
                UserId = userId
            };

            // Loop through each claim we have in our application
            foreach (Claim claim in ClaimsStore.AllClaims)
            {
                UserClaim userClaim = new UserClaim
                {
                    ClaimType = claim.Type
                };

                // If the user has the claim, set IsSelected property to true, so the checkbox
                // next to the claim is checked on the UI
                if (existingUserClaims.Any(c => c.Type == claim.Type && c.Value == "true"))
                {
                    userClaim.IsSelected = true;
                }

                model.Cliams.Add(userClaim);
            }

            return View(model);

        }
        [HttpPost]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.UserId} cannot be found";
                return View("NotFound");
            }

            // Get all the user existing claims and delete them
            var claims = await userManager.GetClaimsAsync(user);
            var result = await userManager.RemoveClaimsAsync(user, claims);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing claims");
                return View(model);
            }

            // Add all the claims that are selected on the UI
            result = await userManager.AddClaimsAsync(user,
                model.Cliams.Select(c => new Claim(c.ClaimType, c.IsSelected ? "true": "false")));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected claims to user");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = model.UserId });

        }
        [HttpGet]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            ViewBag.userId = userId;

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRolesViewModel>();

            foreach (var role in roleManager.Roles)
            {
                var userRolesViewModel = new UserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };

                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.IsSelected = true;
                }
                else
                {
                    userRolesViewModel.IsSelected = false;
                }

                model.Add(userRolesViewModel);
            }

            return View(model);
        }
        [HttpPost]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult>ManageUserRoles(List<UserRolesViewModel> model, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            var roles = await userManager.GetRolesAsync(user);
            var result = await userManager.RemoveFromRolesAsync(user, roles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }

            result = await userManager.AddToRolesAsync(user,
                model.Where(x => x.IsSelected).Select(y => y.RoleName));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = userId });
        }

        [HttpPost]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                var result = await userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View("ListUsers");
            }
        }
        [HttpPost]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                // Wrap the code in a try/catch block
                try
                {
                    //throw new Exception("Test Exception");

                    var result = await roleManager.DeleteAsync(role);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View("ListRoles");
                }
                // If the exception is DbUpdateException, we know we are not able to
                // delete the role as there are users in the role being deleted
                catch (DbUpdateException ex)
                {
                    //Log the exception to a file. We discussed logging to a file
                    // using Nlog in Part 63 of ASP.NET Core tutorial
                    //logger.LogError($"Exception Occured : {ex}");
                    // Pass the ErrorTitle and ErrorMessage that you want to show to
                    // the user using ViewBag. The Error view retrieves this data
                    // from the ViewBag and displays to the user.
                    logger.LogError($"Error deleting role {ex}");
                    ViewBag.ErrorTitle = $"{role.Name} role is in use";
                    ViewBag.ErrorMessage = $"{role.Name} role cannot be deleted as there are users" +
                        $" in this role. If you want to delete this role, please remove the users" +
                        $" from the role and then try to delete";
                   
                    return View("Error");
                }
            }
        }
        [HttpGet]
        [Authorize(Roles = "Super Admin")]
        public IActionResult ListUsers()
        {
            var users = userManager.Users;
            return View(users);
        }

        [HttpGet]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            // GetClaimsAsync retunrs the list of user Claims
            var userClaims = await userManager.GetClaimsAsync(user);
            // GetRolesAsync returns the list of user Roles
            var userRoles = await userManager.GetRolesAsync(user);
            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Claims = userClaims.Select(c =>c.Type +" : "+ c.Value).ToList(),
                Roles = userRoles
            };
            return View(model);
        }
        [HttpPost]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                user.Email = model.Email;
                user.UserName = model.UserName;

                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }
        [HttpGet]
        [Authorize(Roles = "Super Admin")]
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Super Admin")]
        public async Task <IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };
                IdentityResult result = await roleManager.CreateAsync(identityRole);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Administrator");
                }
                foreach(IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("",error.Description);
                }
            }

            return View();
        }
        [HttpGet]
        [Authorize(Roles = "Super Admin")]
        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }


   
     

        [HttpGet]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if(role == null)
            {
                ViewBag.ErrorMessage = $"Role id :{id} cannot be found";
                return View("NotFound");
            }
            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            foreach(var user in userManager.Users)
            {
                if(await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role id :{model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
          
            
        }
        [HttpGet]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;
            var role = await roleManager.FindByIdAsync(roleId);
            if(role == null)
            {
                ViewBag.ErrorMessage = $"Role Id {roleId} cannot be found";
                return View("NotFound");
            }
            var model = new List<UserRoleViewModel>();
            foreach(var user in userManager.Users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }
                model.Add(userRoleViewModel);
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model,string roleId)
        
        {
           var role = await roleManager.FindByIdAsync(roleId);
            if(role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");

            }
            for(int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);
                IdentityResult? result = null;
                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user,role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await userManager.RemoveFromRoleAsync(user,role.Name);
                }
                else
                {
                    continue;
                }
                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("EditRole",new { Id = roleId });
                }
            }
            return RedirectToAction("EditRole",new{ Id = roleId });
        }


        [Authorize(Roles = "Super Admin,Checking,Admin")]
        public async Task<IActionResult> Track(int Id, string SearchCTNum, string SearchBankName, string SearchPreparationAuthority, string SearchDepotName, DateTime? startDate, DateTime? endDate)
        {
            // Use LINQ to get a list of all active items in the 'MainClass' entity set.
            var searchData = _context.Trackers.AsQueryable();

            // Apply other search criteria as needed
            if (!string.IsNullOrEmpty(SearchCTNum))
            {
                searchData = searchData.Where(g => g.CTnumber.Equals(SearchCTNum));
            }

            if (!string.IsNullOrEmpty(SearchBankName))
            {
                searchData = searchData.Where(g => g.BankName.Equals(SearchBankName));
            }

            if (!string.IsNullOrEmpty(SearchPreparationAuthority))
            {
                searchData = searchData.Where(g => g.PreparationAuthority.Equals(SearchPreparationAuthority));
            }

            if (!string.IsNullOrEmpty(SearchDepotName))
            {
                searchData = searchData.Where(g => g.WhoEdit.Equals(SearchDepotName));
            }

            // If a start date for the search has been provided, filter the search data to only include items with a date on or after the start date.
            if (startDate != null)
            {
                DateTime startDateTime = startDate.Value.Date;
                searchData = searchData.Where(x => x.DateOfEdit >= startDateTime);
            }

            // If an end date for the search has been provided, filter the search data to only include items with a date before the day after the end date (to include items on the end date).
            if (endDate != null)
            {
                var nextDay = endDate.Value.Date.AddDays(1);
                searchData = searchData.Where(e => e.DateOfEdit < nextDay);
            }

            ViewBag.SearchCTNum = SearchCTNum; // Pass the search query value to the view
            ViewBag.SearchBankName = SearchBankName; // Pass the location query value to the view
            ViewBag.SearchPreparationAuthority = SearchPreparationAuthority; // Pass the vinNo query value to the view
            ViewBag.SearchDepotName = SearchDepotName; // Pass the fromDate query value to the view

            return View(await searchData.ToListAsync());
        }

      



        // GET: MainClasses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Trackers == null)
            {
                return NotFound();
            }

            var trakers = await _context.Trackers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trakers == null)
            {
                return NotFound();
            }

            return View(trakers);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trakers = await _context.Trackers.FindAsync(id);

            string userName1 = HttpContext.User.Identity.Name;
            trakers.WhoDelet = userName1;
            trakers.DateOfDelet = DateTime.Now;
            if (trakers != null)
            {
                _context.Remove(trakers);
                await _context.SaveChangesAsync();
                TempData["success"] = "تم الحذف بنجاح";
            }
            
            return RedirectToAction(nameof(Track));
        }

        [HttpGet]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> ChangePassword(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            var model = new ChangePasswordViewModel
            {
                UserId = userId
            };

            return View(model);
        }
        [HttpPost]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(model.UserId);

                if (user == null)
                {
                    ViewBag.ErrorMessage = $"User with Id = {model.UserId} cannot be found";
                    return View("Hello");
                }

                var changePasswordResult = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                if (changePasswordResult.Succeeded)
                {
                    // Password changed successfully
                    return RedirectToAction("EditUser", new { Id = model.UserId });
                }

                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            // If we reach here, something went wrong, redisplay the form
            return View(model);
        }



    }
}
