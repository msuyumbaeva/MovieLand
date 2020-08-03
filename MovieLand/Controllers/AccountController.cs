using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MovieLand.BLL.Dtos.User;
using MovieLand.BLL.Enums;
using MovieLand.BLL.Services;
using MovieLand.ViewModels.Account;

namespace MovieLand.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppSignInManager _signInManager;
        private readonly AppUserManager _userManager;
        private readonly IMapper _mapper;

        public AccountController(AppSignInManager signInManager, AppUserManager userManager, IMapper mapper) {
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                await _signInManager.SignOutAsync();
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                    return RedirectToLocal(returnUrl);
                else
                    ModelState.AddModelError("", "Invalid cridentials");
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model) {
            if (ModelState.IsValid) {
                // Map to userDto
                var user = _mapper.Map<UserDto>(model);
                user.Roles.Add(RoleEnum.USER.ToString());
                // Add user
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded) {
                    // Sign in
                    await _signInManager.PasswordSignInAsync(user.UserName,user.Password, false, false);
                    return RedirectToAction("Index", "Home");
                }
                else {
                    foreach (var error in result.Errors) {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        #region Helpers        

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion

    }
}