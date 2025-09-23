using GraduationQRSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraduationQRSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var totalSeniors = await _context.Seniors.CountAsync();
            var totalGuests = await _context.Guests.CountAsync();
            var attendedGuests = await _context.Guests.CountAsync(g => g.IsAttended);
            
            ViewBag.TotalSeniors = totalSeniors;
            ViewBag.TotalGuests = totalGuests;
            ViewBag.AttendedGuests = attendedGuests;
            ViewBag.AttendanceRate = totalGuests > 0 ? Math.Round((double)attendedGuests / totalGuests * 100, 1) : 0;

            return View();
        }
    }
}
