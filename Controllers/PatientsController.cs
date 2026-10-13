using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediCore.Data;
using MediCore.Models;

namespace MediCore.Controllers
{
    public class PatientsController : Controller
    {
        private readonly AppDbContext _context;

        public PatientsController(AppDbContext context)
        {
            _context = context;
        }

        // Vista 2: Listado de Pacientes con Buscador
        public async Task<IActionResult> Index(string searchString)
        {
            var patients = from p in _context.Patients
                           select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                patients = patients.Where(s => s.Name.Contains(searchString) || s.DocumentNumber.Contains(searchString));
            }

            return View(await patients.ToListAsync());
        }

        // Vista 3: Detalle e Historia Clínica del Paciente
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var patient = await _context.Patients
                .Include(p => p.Appointments)
                .Include(p => p.MedicalRecords)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (patient == null) return NotFound();

            return View(patient);
        }

        // GET: Patients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Patients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Patient patient)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(patient);
        }

    }
}