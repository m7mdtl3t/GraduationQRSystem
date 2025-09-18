using Microsoft.AspNetCore.Mvc;

namespace GraduationQRSystem.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Home()
        {
            var username = HttpContext.Session.GetString("Username");
            if (username != "user")
            {
                return RedirectToAction("Login", "Account");
            }
            
            return View();
        }
    }
}
