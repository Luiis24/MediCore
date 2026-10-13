using System.ComponentModel.DataAnnotations;

namespace MediCore.Models
{
    public class MedicalRecord
    {
        public int Id { get; set; }

        public int PatientId { get; set; }
        public Patient? Patient { get; set; }

        public DateTime RecordDate { get; set; } = DateTime.Now;

        [Required]
        public string Notes { get; set; } = string.Empty; // Corregido aquí

        public string DoctorName { get; set; } = "Dr. House";
    }
}