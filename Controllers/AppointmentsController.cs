using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediCore.Data;

namespace MediCore.Controllers
{
    [Route("api/appointments")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AppointmentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/appointments
        [HttpGet]
        public async Task<IActionResult> GetAppointments()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .ToListAsync();
            return Ok(appointments);
        }

        // GET: api/appointments/stats (Ideal para alimentar métricas del Dashboard)
        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var totalPatients = await _context.Patients.CountAsync();
            var activePatients = await _context.Patients.CountAsync(p => p.Status == "Activo");
            var totalAppointments = await _context.Appointments.CountAsync();
            
            // Citas simuladas para hoy (puedes ajustar según la fecha que pusiste en el seeder)
            var todayAppointments = await _context.Appointments
                .CountAsync(a => a.AppointmentDate.Date == DateTime.Today);

            return Ok(new
            {
                TotalPatients = totalPatients,
                ActivePatients = activePatients,
                TotalAppointments = totalAppointments,
                TodayAppointments = todayAppointments
            });
        }
    }
}