using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediSchedule_Prototipo.Models
{
  
    /// Representa un centro o puesto de salud registrado en MediSchedule.
    /// Cada puesto puede tener su propio Admin y personal médico.
  
    public class PuestoSalud
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(300)]
        public string Direccion { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Telefono { get; set; } = string.Empty;

        [MaxLength(100)]
        public string CorreoContacto { get; set; } = string.Empty;

       
        /// Indica si el puesto está activo en el sistema.
       
        public bool EstadoActivo { get; set; } = true;

        
        /// Relación: Un Puesto de Salud tiene muchos usuarios (Admins, PersonalMedico, Pacientes asociados).
        
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();

        
        /// Constructor limpio, usado por EF Core.
      
        public PuestoSalud() { }

        public PuestoSalud(string nombre, string direccion, string telefono, string correo)
        {
            Nombre = nombre;
            Direccion = direccion;
            Telefono = telefono;
            CorreoContacto = correo;
            EstadoActivo = true;
        }
    }
}

