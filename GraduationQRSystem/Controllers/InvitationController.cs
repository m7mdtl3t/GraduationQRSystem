using GraduationQRSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraduationQRSystem.Controllers
{
    public class InvitationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InvitationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Show(int id)
        {
            var senior = await _context.Seniors.AsNoTracking().FirstOrDefaultAsync(s => s.SeniorId == id);
            if (senior == null) return NotFound();
            return View(senior);
        }
    }
}


