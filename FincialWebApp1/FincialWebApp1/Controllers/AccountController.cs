using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FincialWebApp1.ViewModels;

namespace FincialWebApp1.Controllers
{
    //[AllowAnonymous]
    [Authorize(Roles = "Super Admin")]

    public class AccountController : Microsoft.AspNetCore.Mvc.Controller 
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public AccountController(UserManager<IdentityUser> userManager,
                                    SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public UserManager<IdentityUser> UserManager { get; }
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "home");
        }
        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> IsEmailInUse   (string email)
        {
            var user = await userManager.FindByNameAsync(email);
            if(user != null)
            {
                return Json(true);
            }
            else
            {
                return Json($"email {email} is already taken");
            }
        }

        [HttpGet]
       [Authorize(Roles = "Super Admin")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Super Admin")] 
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    
                };

                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // If the user is signed in and in the Admin role, then it is
                    // the Admin user that is creating a new user. So redirect the
                    // Admin user to ListRoles action

                    if(signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListUsers", "Administrator");
                    }
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
           
        }



        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model,string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email,
                    model.Password,model.RememberMe,false);
                if (result.Succeeded)
                {
                   
                        return RedirectToAction("Index", "Home");
                    
                }
                
                    ModelState.AddModelError(string.Empty, "عملية تسجيل خاظئة");
                
            }  
            return View(model);
        }
        [Authorize]
        public  IActionResult LogInConfirm()
        {
            return View();
        }

    }
}
