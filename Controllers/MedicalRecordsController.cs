using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediCore.Data;

namespace MediCore.Controllers
{
    [Route("api/medical-records")]
    [ApiController]
    public class MedicalRecordsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MedicalRecordsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/medical-records/patient/{patientId} (Obtiene la historia clínica de un paciente específico)
        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetRecordsByPatient(int patientId)
        {
            var records = await _context.MedicalRecords
                .Where(r => r.PatientId == patientId)
                .OrderByDescending(r => r.RecordDate)
                .ToListAsync();

            if (records == null || !records.Any())
            {
                return NotFound(new { message = "No se encontraron registros clínicos para este paciente." });
            }

            return Ok(records);
        }
    }
}