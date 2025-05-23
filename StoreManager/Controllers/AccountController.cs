using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StoreManager.Features.EmailHelper;
using StoreManager.Models;
using StoreManager.ViewModels;

namespace StoreManager.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailHelper _emailHelper;
        public AccountController(SignInManager<AppUser> signInManager, ILogger<AccountController> logger, IEmailHelper emailHelper)
        {
            _signInManager = signInManager;
            _logger = logger;
            _emailHelper = emailHelper;

        }
        public IActionResult Login()
        {
            return View();

        }
        public async Task<IActionResult> AccessDenied()
        {
            await _emailHelper.SendUnauthorizedEmailAsync();
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username!, model.Password!, model.RememberMe, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Invalid Login attempt");
                _logger.LogWarning("Invalid Login attempt");
                return View(model);
            }
            return View(model);
        }
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
