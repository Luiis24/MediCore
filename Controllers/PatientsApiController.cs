using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediCore.Data;
using MediCore.Models;

[Route("api/patients")]
[ApiController]
public class PatientsApiController : ControllerBase
{
    private readonly AppDbContext _context;

    public PatientsApiController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/patients
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Patient>>> GetPatients()
    {
        return await _context.Patients.ToListAsync();
    }

    // POST: api/patients
    [HttpPost]
    public async Task<ActionResult<Patient>> PostPatient(Patient patient)
    {
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPatients), new { id = patient.Id }, patient);
    }
}