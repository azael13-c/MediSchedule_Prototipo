using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediSchedule_Prototipo.Models
{
    
    /// Representa una cita médica entre un Paciente y un PersonalMedico.
   
    public class Cita
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Relación con Paciente
        [Required]
        public int PacienteId { get; set; }
        public Usuario Paciente { get; set; } = null!;

        // Relación con PersonalMedico
        [Required]
        public int PersonalMedicoId { get; set; }
        public Usuario PersonalMedico { get; set; } = null!;

        [Required]
        public DateTime Fecha { get; set; }

        [MaxLength(500)]
        public string Observaciones { get; set; } = string.Empty;

        public bool EstadoActivo { get; set; } = true;

        // Constructor vacío para EF Core
        public Cita() { }

        // Constructor completo
        public Cita(int pacienteId, int personalMedicoId, DateTime fecha, string observaciones = "")
        {
            PacienteId = pacienteId;
            PersonalMedicoId = personalMedicoId;
            Fecha = fecha;
            Observaciones = observaciones;
            EstadoActivo = true;
        }
    }
}

