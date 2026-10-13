using System.ComponentModel.DataAnnotations;

namespace MediCore.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        public int PatientId { get; set; }
        public Patient? Patient { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; } // Puede ser de hoy o días pasados para gráficos

        public string Status { get; set; } = "Programada"; // Programada, Completada, Cancelada

        public string Reason { get; set; } = string.Empty;
    }
}