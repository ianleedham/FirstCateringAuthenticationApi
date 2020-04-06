using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FirstCateringAuthenticationApi.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AuthenticationController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public Task<IActionResult> Tap(string cardNumber)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> TapWithPin(string cardNumber, int pin)
        {
            throw new NotImplementedException();
        }

        public IActionResult Register()
        {
            throw new NotImplementedException();
        }
    }
}