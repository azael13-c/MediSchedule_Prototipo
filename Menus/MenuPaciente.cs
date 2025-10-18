using MediSchedule_Prototipo.Models;
using MediSchedule_Prototipo.Services;
using System;
using System.Linq;
using System.Threading;

namespace MediSchedule_Prototipo.Menus
{
    public class MenuPaciente
    {
        private readonly CitaService _citaService;
        private readonly UsuarioService _usuarioService;

        public MenuPaciente(CitaService citaService, UsuarioService usuarioService)
        {
            _citaService = citaService;
            _usuarioService = usuarioService;
        }

        #region Menú Principal
        public bool Mostrar(Usuario paciente)
        {
            while (true)
            {
                Console.Clear();
                MostrarEncabezado();

                Console.Write("→ Seleccione una opción: ");
                string opcion = Console.ReadLine()?.Trim() ?? "";

                try
                {
                    switch (opcion)
                    {
                        case "1":
                            VerMisCitas(paciente.Id);
                            break;
                        case "2":
                            VerMedicosDisponibles();
                            break;
                        case "0":
                            Console.WriteLine("\n👋 Cerrando sesión...");
                            Thread.Sleep(1000);
                            return true;
                        default:
                            MostrarAdvertencia("Opción inválida. Intente nuevamente.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    MostrarError($"Error inesperado: {ex.Message}");
                }

                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey(true);
            }
        }
        #endregion

        #region Funciones de Citas
        private void VerMisCitas(int pacienteId)
        {
            Console.Clear();
            MostrarSubtitulo("📅 Mis citas");

            try
            {
                var citas = _citaService.ObtenerCitasPorPaciente(pacienteId).ToList();

                if (!citas.Any())
                {
                    MostrarAdvertencia("No tienes citas registradas actualmente.");
                    return;
                }

                foreach (var c in citas)
                {
                    Console.WriteLine($"🆔 ID: {c.Id} | 👨‍⚕️ Médico: {c.PersonalMedico.Nombre} | 🕓 Fecha: {c.Fecha:g}");
                }
            }
            catch (Exception ex)
            {
                MostrarError($"Error al obtener citas: {ex.Message}");
            }
        }
        #endregion

        #region Funciones de Médicos
        private void VerMedicosDisponibles()
        {
            Console.Clear();
            MostrarSubtitulo("👩‍⚕️ Médicos disponibles");

            try
            {
                var medicos = _usuarioService.ListarPorRol(Rol.PersonalMedico).ToList();

                if (!medicos.Any())
                {
                    MostrarAdvertencia("No hay médicos disponibles en este momento.");
                    return;
                }

                foreach (var m in medicos)
                {
                    string estado = m.EstadoActivo ? "🟢 Activo" : "🔴 Inactivo";
                    Console.WriteLine($"🆔 {m.Id} | {m.Nombre} | Estado: {estado}");
                }
            }
            catch (Exception ex)
            {
                MostrarError($"Error al listar médicos: {ex.Message}");
            }
        }
        #endregion

        #region Utilidades de Presentación y Mensajes
        private static void MostrarEncabezado()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=============================================================");
            Console.WriteLine("              👩‍⚕️ MENÚ PACIENTE — MediSchedule");
            Console.WriteLine("=============================================================");
            Console.ResetColor();

            Console.WriteLine("1️⃣  Ver mis citas");
            Console.WriteLine("2️⃣  Ver médicos disponibles");
            Console.WriteLine("0️⃣  Cerrar sesión");
            Console.WriteLine();
        }

        private static void MostrarSubtitulo(string titulo)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("-------------------------------------------------------------");
            Console.WriteLine(titulo);
            Console.WriteLine("-------------------------------------------------------------");
            Console.ResetColor();
        }

        private static void MostrarError(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ {mensaje}");
            Console.ResetColor();
            Thread.Sleep(1500);
        }

        private static void MostrarAdvertencia(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"⚠ {mensaje}");
            Console.ResetColor();
            Thread.Sleep(1200);
        }
        #endregion
    }
}
