using System.ComponentModel.DataAnnotations;

namespace MediCore.Models
{
    public class Patient
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El documento es obligatorio")]
        public string DocumentNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "La edad es obligatoria")]
        public int Age { get; set; }

        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "El diagnóstico es obligatorio")]
        public string Diagnosis { get; set; } = string.Empty;

        public string Status { get; set; } = "Activo"; // Activo o Inactivo para métricas

        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        // Relaciones
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
    }
}