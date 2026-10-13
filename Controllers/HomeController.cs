using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediCore.Models;
using MediCore.Data;

namespace MediCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        // Vista 1: Dashboard Principal
        public async Task<IActionResult> Index()
        {
            ViewBag.TotalPatients = await _context.Patients.CountAsync();
            ViewBag.ActivePatients = await _context.Patients.CountAsync(p => p.Status == "Activo");
            ViewBag.TotalAppointments = await _context.Appointments.CountAsync();
            ViewBag.TodayAppointments = await _context.Appointments.CountAsync(a => a.AppointmentDate.Date == DateTime.Today);

            var recentAppointments = await _context.Appointments
                .Include(a => a.Patient)
                .OrderByDescending(a => a.AppointmentDate)
                .Take(5)
                .ToListAsync();

            return View(recentAppointments);
        }

        // GET: Home/CreateAppointment
        public async Task<IActionResult> CreateAppointment()
        {
            ViewBag.Patients = await _context.Patients.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAppointment(Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Patients = await _context.Patients.ToListAsync();
            return View(appointment);
        }

        // POST: Home/UpdateAppointmentStatus
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                appointment.Status = status;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}