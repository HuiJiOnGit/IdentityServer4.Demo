using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mvc.Client.Controllers
{
    public class AccountController : Controller
    {
        [Authorize]
        public IActionResult RemoteLogin()
        {
            return RedirectToAction(nameof(Index), nameof(HomeController).Replace("Controller",""));
        }

        public IActionResult Logout()
        {
            return SignOut(CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}