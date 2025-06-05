using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplication1.Models; 

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Username };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    
                    if (model.Username.Equals("admin", StringComparison.OrdinalIgnoreCase)) 
                    {
                        if (await _userManager.FindByNameAsync("admin") != null) 
                        {
                            if (!await HttpContext.RequestServices.GetRequiredService<RoleManager<IdentityRole>>().RoleExistsAsync("Admin"))
                            {
                                await HttpContext.RequestServices.GetRequiredService<RoleManager<IdentityRole>>().CreateAsync(new IdentityRole("Admin"));
                            }
                            await _userManager.AddToRoleAsync(user, "Admin");
                            _logger.LogInformation($"User {user.UserName} added to Admin role.");
                        }
                    }



                    TempData["SuccessMessage"] = "Registration successful! You can now log in.";
                    return RedirectToAction("Login", "Account"); 
                }
                AddErrors(result);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl; 
            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                returnUrl = Url.Content("~/"); 
            }
           
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");

                    HttpContext.Session.SetString("Username", model.Username);
                    HttpContext.Session.SetInt32("Funds", 100000); 

                    var user = await _userManager.FindByNameAsync(model.Username);
                    if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        HttpContext.Session.SetString("IsAdmin", "true");
                        HttpContext.Session.SetInt32("Funds", (int)Math.Floor(user.Balance));

                    }
                    else
                    {
                        HttpContext.Session.Remove("IsAdmin");
                    }

                    return LocalRedirect(returnUrl);
                }

                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            return View(model);
        }
        [HttpPost]
        [Authorize] 
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deposit(DepositViewModel model) 
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                user.Balance += model.Amount;

                var updateResult = await _userManager.UpdateAsync(user);
                if (updateResult.Succeeded)
                {
                    _logger.LogInformation($"User '{user.UserName}' deposited {model.Amount:C}. New balance: {user.Balance:C}.");
                    TempData["SuccessMessage"] = $"Successfully deposited {model.Amount:C}. Your new balance is {user.Balance:C}.";

                }
                else
                {
                    TempData["ErrorMessage"] = "There was an error processing your deposit. Please try again.";
                    foreach (var error in updateResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                        _logger.LogError($"Error updating user balance for '{user.UserName}': {error.Description}");
                    }
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["ErrorMessage"] = "Invalid deposit amount. " + string.Join(" ", errors);
            }

            string returnUrl = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home"); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            HttpContext.Session.Clear(); 
            return RedirectToAction("Index", "Home");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}