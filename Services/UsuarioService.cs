using MediSchedule_Prototipo.Data;
using MediSchedule_Prototipo.Models;
using System;
using System.Linq;
using System.Collections.Generic;

namespace MediSchedule_Prototipo.Services
{
    public class UsuarioService
    {
        private readonly MediScheduleContext _context;

        public UsuarioService(MediScheduleContext context)
        {
            _context = context;
        }

        public Usuario CrearUsuario(string nombre, string contrasena, Rol rol, int? puestoSaludId = null)
        {
            if (string.IsNullOrWhiteSpace(nombre) || nombre.Length < 3)
                throw new ArgumentException("El nombre debe tener al menos 3 caracteres.");

            if (_context.Usuarios.Any(u => u.Nombre == nombre))
                throw new ArgumentException("Ya existe un usuario con ese nombre.");

            if (string.IsNullOrWhiteSpace(contrasena) || !System.Text.RegularExpressions.Regex.IsMatch(contrasena, @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,}$"))
                throw new ArgumentException("Contraseña inválida. Debe tener 6+ caracteres y al menos una letra y un número.");

            var usuario = new Usuario(nombre, contrasena, rol)
            {
                PuestoSaludId = puestoSaludId
            };

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();
            return usuario;
        }

        public Usuario? ObtenerPorId(int id) => _context.Usuarios.FirstOrDefault(u => u.Id == id);

        public IQueryable<Usuario> ListarTodos() => _context.Usuarios;

        public IQueryable<Usuario> ListarActivos() => _context.Usuarios.Where(u => u.EstadoActivo);

        public IQueryable<Usuario> ListarPorRol(Rol rol) => _context.Usuarios.Where(u => u.Rol == rol);

        public List<Usuario> ListarPorPuesto(int puestoId) =>
            _context.Usuarios
                .Where(u => u.PuestoSaludId == puestoId)
                .OrderBy(u => u.Nombre)
                .ToList();

        public List<Usuario> ListarPacientesPorMedico(int medicoId)
        {
            var medico = ObtenerPorId(medicoId);
            if (medico == null || !medico.PuestoSaludId.HasValue)
                return new List<Usuario>();

            return _context.Usuarios
                .Where(u => u.PuestoSaludId == medico.PuestoSaludId
                            && u.Rol == Rol.Paciente
                            && u.EstadoActivo)
                .ToList();
        }

        public Usuario? Login(int id, string contrasena)
        {
            var usuario = ObtenerPorId(id);
            if (usuario == null) return null;
            if (usuario.EstaBloqueado())
            {
                var bloqueo = usuario.BloqueoHasta.HasValue
                    ? usuario.BloqueoHasta.Value.ToString("yyyy-MM-dd HH:mm")
                    : "desconocido";
                throw new Exception($"Usuario bloqueado hasta {bloqueo}");
            }

            if (!usuario.ValidarContrasena(contrasena))
            {
                usuario.IntentosFallidos++;
                if (usuario.IntentosFallidos >= 3)
                {
                    usuario.BloqueoHasta = DateTime.Now.AddMinutes(5);
                    usuario.IntentosFallidos = 0;
                }
                _context.SaveChanges();
                return null;
            }

            usuario.IntentosFallidos = 0;
            usuario.BloqueoHasta = null;
            _context.SaveChanges();

            return usuario;
        }

        public void DesactivarUsuario(int id)
        {
            var usuario = ObtenerPorId(id) ?? throw new Exception("Usuario no encontrado.");
            usuario.EstadoActivo = false;
            _context.SaveChanges();
        }

        public void ActivarUsuario(int id)
        {
            var usuario = ObtenerPorId(id) ?? throw new Exception("Usuario no encontrado.");
            usuario.EstadoActivo = true;
            _context.SaveChanges();
        }
    }
}
