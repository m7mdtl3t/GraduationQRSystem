using System.Text;
using GraduationQRSystem.Data;
using GraduationQRSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;

namespace GraduationQRSystem.Controllers
{
    public class SeniorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public SeniorsController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }
            
            var seniors = await _context.Seniors.AsNoTracking().ToListAsync();
            return View(seniors);
        }

        [HttpGet]
        public async Task<IActionResult> SearchBySenior(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return Json(new List<object>());
            }

            var seniors = await _context.Seniors
                .Include(s => s.Guests)
                .Where(s => s.Name.ToLower().Contains(term.ToLower()))
                .Select(s => new
                {
                    SeniorId = s.SeniorId,
                    Name = s.Name,
                    NumberOfGuests = s.NumberOfGuests,
                    Guests = s.Guests.Select(g => new
                    {
                        GuestId = g.GuestId,
                        Name = g.Name,
                        IsAttended = g.IsAttended,
                        AttendanceTime = g.AttendanceTime
                    }).ToList()
                })
                .ToListAsync();

            return Json(seniors);
        }

        [HttpGet]
        public async Task<IActionResult> SearchByGuest(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return Json(new List<object>());
            }

            var guests = await _context.Guests
                .Include(g => g.Senior)
                .Where(g => g.Name.ToLower().Contains(term.ToLower()))
                .Select(g => new
                {
                    GuestId = g.GuestId,
                    Name = g.Name,
                    IsAttended = g.IsAttended,
                    AttendanceTime = g.AttendanceTime,
                    Senior = new
                    {
                        SeniorId = g.Senior!.SeniorId,
                        Name = g.Senior.Name,
                        NumberOfGuests = g.Senior.NumberOfGuests
                    }
                })
                .ToListAsync();

            return Json(guests);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,NumberOfGuests,PhoneNumber")] Senior senior)
        {
            if (!ModelState.IsValid) return View(senior);
            _context.Seniors.Add(senior);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = senior.SeniorId });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var senior = await _context.Seniors.FindAsync(id);
            if (senior == null) return NotFound();
            return View(senior);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SeniorId,Name,NumberOfGuests,PhoneNumber")] Senior senior)
        {
            if (id != senior.SeniorId) return NotFound();
            if (!ModelState.IsValid) return View(senior);
            _context.Update(senior);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, bool ajax = false)
        {
            var senior = await _context.Seniors.FindAsync(id);
            if (senior != null)
            {
                _context.Seniors.Remove(senior);
                await _context.SaveChangesAsync();
                
                if (ajax)
                {
                    return Ok(new { success = true, message = "Senior deleted successfully" });
                }
            }
            
            if (ajax)
            {
                return NotFound(new { success = false, message = "Senior not found" });
            }
            
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                // If not logged in, redirect to public invitation view
                return RedirectToAction("Show", "Invitation", new { id });
            }
            
            var senior = await _context.Seniors
                .Include(s => s.Guests)
                .FirstOrDefaultAsync(s => s.SeniorId == id);
            if (senior == null) return NotFound();

            // Data hygiene: if a guest is not attended, ensure attendance time is cleared
            var normalized = false;
            foreach (var g in senior.Guests)
            {
                if (!g.IsAttended && g.AttendanceTime != null)
                {
                    g.AttendanceTime = null;
                    normalized = true;
                }
                else if (g.IsAttended && g.AttendanceTime == null)
                {
                    g.AttendanceTime = DateTime.UtcNow;
                    normalized = true;
                }
            }
            if (normalized)
            {
                await _context.SaveChangesAsync();
            }

            // Build public URL
            var publicHost = _configuration["PublicHost"];
            var host = !string.IsNullOrWhiteSpace(publicHost) ? publicHost : $"http://{Request.Host}";
            var url = $"{host}/Seniors/Details/{id}";
            senior.QrUrl = url;

            return View(senior);
        }

        public async Task<IActionResult> QrImage(int id)
        {
            var senior = await _context.Seniors.FindAsync(id);
            if (senior == null) return NotFound();

            var publicHost = _configuration["PublicHost"];
            var host = !string.IsNullOrWhiteSpace(publicHost) ? publicHost : $"http://{Request.Host}";
            var url = $"{host}/Seniors/Details/{id}";

            using var generator = new QRCodeGenerator();
            using QRCodeData data = generator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            var qrPng = new PngByteQRCode(data);
            byte[] pngBytes = qrPng.GetGraphic(20);

            // Save to disk under wwwroot/qrcodes/<SeniorName>/qr-<id>.png
            var webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var safeName = string.Join("_", (senior.Name ?? $"Senior_{id}").Split(Path.GetInvalidFileNameChars()));
            var folder = Path.Combine(webRoot, "qrcodes", safeName);
            Directory.CreateDirectory(folder);
            var downloadName = $"{safeName}.png";
            var filePath = Path.Combine(folder, downloadName);
            await System.IO.File.WriteAllBytesAsync(filePath, pngBytes);

            return File(pngBytes, "image/png", downloadName);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAttendance(int guestId)
        {
            var guest = await _context.Guests.FindAsync(guestId);
            if (guest == null) return NotFound();

            guest.IsAttended = true;
            guest.AttendanceTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var seniorId = guest.SeniorId;
            return RedirectToAction(nameof(Details), new { id = seniorId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnmarkAttendance(int guestId)
        {
            var guest = await _context.Guests.FindAsync(guestId);
            if (guest == null) return NotFound();

            guest.IsAttended = false;
            guest.AttendanceTime = null;
            await _context.SaveChangesAsync();

            var seniorId = guest.SeniorId;
            return RedirectToAction(nameof(Details), new { id = seniorId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddGuest(int seniorId, string name, string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(phoneNumber))
            {
                TempData["GuestError"] = "Name and phone number are required.";
                return RedirectToAction(nameof(Details), new { id = seniorId });
            }

            var senior = await _context.Seniors.Include(s => s.Guests).FirstOrDefaultAsync(s => s.SeniorId == seniorId);
            if (senior == null)
            {
                return NotFound();
            }

            var currentGuests = senior.Guests.Count;
            if (currentGuests >= senior.NumberOfGuests)
            {
                TempData["GuestLimit"] = $"Guest limit reached ({senior.NumberOfGuests}).";
                return RedirectToAction(nameof(Details), new { id = seniorId });
            }

            _context.Guests.Add(new Guest { Name = name.Trim(), PhoneNumber = phoneNumber.Trim(), SeniorId = seniorId });
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = seniorId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGuest(int id)
        {
            var guest = await _context.Guests.FindAsync(id);
            if (guest == null) return NotFound();
            var seniorId = guest.SeniorId;
            _context.Guests.Remove(guest);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = seniorId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGuest(int id, string name, string phoneNumber)
        {
            var guest = await _context.Guests.FindAsync(id);
            if (guest == null) return NotFound();
            var seniorId = guest.SeniorId;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(phoneNumber))
            {
                TempData["GuestEditError"] = "Guest name is required.";
                return RedirectToAction(nameof(Details), new { id = seniorId });
            }

            guest.Name = name.Trim();
            guest.PhoneNumber = phoneNumber.Trim();
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = seniorId });
        }
    }
}


