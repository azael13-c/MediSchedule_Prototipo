using MediSchedule_Prototipo.Data;
using MediSchedule_Prototipo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MediSchedule_Prototipo.Services
{
    
    /// Servicio para manejo de citas médicas.
    /// Contiene creación y consultas de citas con relaciones cargadas.
  
    public class CitaService
    {
        private readonly MediScheduleContext _context;

        public CitaService(MediScheduleContext context)
        {
            _context = context;
        }

        
        /// Crea una nueva cita según el rol del usuario creador.
        /// - PersonalMedico solo puede crear citas para sí mismo.
        /// - Admin y SuperAdmin pueden crear para cualquier médico.
       
        public Cita CrearCita(Usuario usuarioCreador, int pacienteId, int personalMedicoId, DateTime fecha, string observaciones)
        {
            var paciente = _context.Usuarios.FirstOrDefault(u => u.Id == pacienteId && u.Rol == Rol.Paciente);
            if (paciente == null) throw new Exception("Paciente no encontrado.");

            var medico = _context.Usuarios.FirstOrDefault(u => u.Id == personalMedicoId && u.Rol == Rol.PersonalMedico);
            if (medico == null) throw new Exception("Personal médico no encontrado.");

            // Validación según rol del creador
            if (usuarioCreador.Rol == Rol.PersonalMedico && usuarioCreador.Id != personalMedicoId)
                throw new Exception("Un Personal Médico solo puede crear citas para sí mismo.");

            var cita = new Cita
            {
                PacienteId = pacienteId,
                PersonalMedicoId = personalMedicoId,
                Fecha = fecha,
                Observaciones = observaciones
            };

            _context.Citas.Add(cita);
            _context.SaveChanges();

            return cita;
        }

       
        /// Obtiene todas las citas con relaciones cargadas (Paciente y PersonalMedico)
        
        public List<Cita> ObtenerTodas()
        {
            return _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.PersonalMedico)
                .Include(c => c.Paciente.PuestoSalud)
                .ToList();
        }

        
        /// Obtiene todas las citas de un paciente
       
        public IQueryable<Cita> ObtenerCitasPorPaciente(int pacienteId)
        {
            return _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.PersonalMedico)
                .Where(c => c.PacienteId == pacienteId);
        }

        
        /// Obtiene todas las citas de un médico
        
        public IQueryable<Cita> ObtenerCitasPorMedico(int personalMedicoId)
        {
            return _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.PersonalMedico)
                .Where(c => c.PersonalMedicoId == personalMedicoId);
        }

        
        /// Obtiene todas las citas de un puesto de salud específico
       
        public List<Cita> ObtenerCitasPorPuesto(int puestoId)
        {
            return _context.Citas
                .Include(c => c.Paciente)
                .Include(c => c.PersonalMedico)
                .Where(c => c.Paciente.PuestoSaludId == puestoId)
                .ToList();
        }

        
        /// Listar pacientes asignados a un médico
       
        public List<Usuario> ListarPacientesPorMedico(int personalMedicoId)
        {
            return _context.Citas
                .Include(c => c.Paciente)
                .Where(c => c.PersonalMedicoId == personalMedicoId)
                .Select(c => c.Paciente)
                .Distinct()
                .ToList();
        }
    }
}
