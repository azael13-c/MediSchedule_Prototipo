using MediSchedule_Prototipo.Data;
using MediSchedule_Prototipo.Models;
using System;
using System.Linq;

namespace MediSchedule_Prototipo.Services
{
   
    /// Servicio para manejo de Puestos de Salud.
    /// Permite crear, listar, activar y desactivar puestos.
    
    public class PuestoSaludService
    {
        private readonly MediScheduleContext _context;

        public PuestoSaludService(MediScheduleContext context)
        {
            _context = context;
        }

        
        /// Crear un nuevo puesto de salud
       
        public PuestoSalud CrearPuesto(string nombre, string direccion = "", string telefono = "", string correo = "")
        {
            var puesto = new PuestoSalud(nombre, direccion, telefono, correo);
            _context.PuestosSalud.Add(puesto);
            _context.SaveChanges();
            return puesto;
        }

        
        /// Listar todos los puestos de salud (incluyendo inactivos)
       
        public IQueryable<PuestoSalud> ListarTodos()
        {
            return _context.PuestosSalud;
        }

        
        /// Listar solo puestos activos
       
        public IQueryable<PuestoSalud> ListarActivos()
        {
            return _context.PuestosSalud.Where(p => p.EstadoActivo);
        }

       
        /// Obtener un puesto por ID
        
        public PuestoSalud? ObtenerPorId(int id)
        {
            return _context.PuestosSalud.FirstOrDefault(p => p.Id == id);
        }

       
        /// Desactivar un puesto de salud
      
        public void DesactivarPuesto(int id)
        {
            var puesto = ObtenerPorId(id);
            if (puesto == null)
                throw new Exception("Puesto de salud no encontrado.");

            puesto.EstadoActivo = false;
            _context.SaveChanges();
        }

        
        /// Reactivar un puesto de salud
        
        public void ActivarPuesto(int id)
        {
            var puesto = ObtenerPorId(id);
            if (puesto == null)
                throw new Exception("Puesto de salud no encontrado.");

            puesto.EstadoActivo = true;
            _context.SaveChanges();
        }
    }
}
