using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MediSchedule_Prototipo.Utils;

namespace MediSchedule_Prototipo.Models
{
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string ContrasenaHash { get; private set; } = string.Empty;

        [Required]
        public Rol Rol { get; set; }

        public bool EstadoActivo { get; set; } = true;

        public int? PuestoSaludId { get; set; }
        public PuestoSalud? PuestoSalud { get; set; }

        public int IntentosFallidos { get; set; } = 0;
        public DateTime? BloqueoHasta { get; set; } = null;

        public bool EstaBloqueado() => BloqueoHasta.HasValue && DateTime.Now < BloqueoHasta.Value;

        public Usuario() { }

        /// Constructor seguro para crear usuarios
        public Usuario(string nombre, string contrasena, Rol rol)
        {
            Nombre = nombre;
            Rol = rol;
            EstadoActivo = true;

            // Generamos un email único automático
            Email = $"{nombre.ToLower()}{Guid.NewGuid():N}@medischedule.local";

            SetContrasena(contrasena);
        }

        public void SetContrasena(string contrasena)
        {
            ContrasenaHash = Seguridad.GenerarHash(contrasena);
        }

        public bool ValidarContrasena(string contrasena)
        {
            return Seguridad.VerificarHash(contrasena, ContrasenaHash);
        }
    }
}
