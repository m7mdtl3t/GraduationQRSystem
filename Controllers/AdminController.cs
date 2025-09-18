using Microsoft.AspNetCore.Mvc;

namespace GraduationQRSystem.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Home()
        {
            var username = HttpContext.Session.GetString("Username");
            if (username != "admin")
            {
                return RedirectToAction("Login", "Account");
            }
            
            return View();
        }
    }
}
