using Microsoft.AspNetCore.Mvc;

namespace GraduationQRSystem.Controllers
{
    public class AccountController : Controller
    {
        // In-Memory users
        private static readonly Dictionary<string, string> users = new()
        {
            {"admin", "123"},
            {"user", "123"}
        };

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Please enter username and password";
                return View();
            }

            if (users.ContainsKey(username) && users[username] == password)
            {
                // Set session
                HttpContext.Session.SetString("Username", username);
                HttpContext.Session.SetString("Role", username == "admin" ? "Admin" : "User");

                // Redirect based on role
                if (username == "admin")
                {
                    return RedirectToAction("Home", "Admin");
                }
                else
                {
                    return RedirectToAction("Home", "User");
                }
            }

            ViewBag.Error = "Invalid login";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
