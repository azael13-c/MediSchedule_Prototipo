using MediSchedule_Prototipo.Models;
using MediSchedule_Prototipo.Services;
using System;
using System.Linq;
using System.Threading;

namespace MediSchedule_Prototipo.Menus
{
    public class MenuPersonalMedico
    {
        private readonly CitaService _citaService;
        private readonly UsuarioService _usuarioService;

        public MenuPersonalMedico(CitaService citaService, UsuarioService usuarioService)
        {
            _citaService = citaService;
            _usuarioService = usuarioService;
        }

        #region Menú principal
        public bool Mostrar(Usuario medico)
        {
            while (true)
            {
                Console.Clear();
                MostrarTitulo("👨‍⚕️ MENÚ PERSONAL MÉDICO");

                Console.WriteLine("1️⃣  Crear cita (para mí)");
                Console.WriteLine("2️⃣  Listar mis citas");
                Console.WriteLine("3️⃣  Ver pacientes asignados");
                Console.WriteLine("0️⃣  Cerrar sesión");
                Console.Write("\n→ Opción: ");

                string op = Console.ReadLine()?.Trim() ?? "";

                try
                {
                    switch (op)
                    {
                        case "1":
                            CrearCita(medico);
                            break;
                        case "2":
                            ListarCitas(medico.Id);
                            break;
                        case "3":
                            ListarPacientesAsignados(medico.Id);
                            break;
                        case "0":
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
            }
        }
        #endregion

        #region Crear Cita
        private void CrearCita(Usuario medico)
        {
            Console.Clear();
            MostrarTitulo("🩺 CREAR CITA");

            // --- Mostrar pacientes disponibles ---
            var pacientes = _usuarioService.ListarTodos()
                .Where(u => u.Rol == Rol.Paciente && u.EstadoActivo)
                .ToList();

            if (!pacientes.Any())
            {
                MostrarAdvertencia("No hay pacientes activos disponibles.");
                return;
            }

            Console.WriteLine("\nPacientes disponibles:");
            foreach (var p in pacientes)
                Console.WriteLine($"ID:{p.Id,-3} | {p.Nombre}");

            // --- Leer paciente ---
            int pacId = LeerNumero("ID del paciente");
            if (pacId == -1 || !pacientes.Any(p => p.Id == pacId))
            {
                MostrarAdvertencia("Paciente no válido o no existe.");
                return;
            }

            // --- Leer fecha ---
            DateTime fecha;
            while (true)
            {
                Console.Write("Fecha (YYYY-MM-DD HH:MM): ");
                string input = Console.ReadLine()?.Trim() ?? "";
                if (DateTime.TryParse(input, out fecha) && fecha > DateTime.Now)
                    break;

                MostrarAdvertencia("Formato de fecha inválido o fecha pasada. Intente nuevamente.");
            }

            // --- Leer observaciones ---
            Console.Write("Observaciones (opcional): ");
            string obs = Console.ReadLine()?.Trim() ?? "";

            // --- Confirmación ---
            if (!ConfirmarAccion("¿Desea crear esta cita?"))
            {
                MostrarAdvertencia("Acción cancelada.");
                return;
            }

            // --- Crear cita ---
            try
            {
                var cita = _citaService.CrearCita(medico, pacId, medico.Id, fecha, obs);
                MostrarExito($"✅ Cita creada exitosamente con ID: {cita.Id}");
            }
            catch (Exception ex)
            {
                MostrarError($"Error al crear cita: {ex.Message}");
            }

            EsperarContinuar();
        }
        #endregion

        #region Listar Citas
        private void ListarCitas(int medicoId)
        {
            Console.Clear();
            MostrarTitulo("📅 MIS CITAS");

            try
            {
                var citas = _citaService.ObtenerCitasPorMedico(medicoId)
                    .OrderBy(c => c.Fecha)
                    .ToList();

                if (!citas.Any())
                {
                    MostrarAdvertencia("No tienes citas registradas.");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("ID".PadRight(6) + "Paciente".PadRight(20) + "Fecha".PadRight(20) + "Observaciones");
                    Console.WriteLine(new string('-', 70));
                    Console.ResetColor();

                    foreach (var c in citas)
                    {
                        string pac = c.Paciente?.Nombre ?? "—";
                        Console.WriteLine($"{c.Id.ToString().PadRight(6)}{pac.PadRight(20)}{c.Fecha:yyyy-MM-dd HH:mm}  {c.Observaciones}");
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarError($"Error al obtener citas: {ex.Message}");
            }

            EsperarContinuar();
        }
        #endregion

        #region Pacientes Asignados
        private void ListarPacientesAsignados(int medicoId)
        {
            Console.Clear();
            MostrarTitulo("👥 PACIENTES ASIGNADOS");

            try
            {
                var pacientes = _usuarioService.ListarPacientesPorMedico(medicoId).ToList();

                if (!pacientes.Any())
                {
                    MostrarAdvertencia("No tienes pacientes asignados actualmente.");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("ID".PadRight(6) + "Nombre".PadRight(25) + "Activo");
                    Console.WriteLine(new string('-', 45));
                    Console.ResetColor();

                    foreach (var p in pacientes)
                    {
                        Console.ForegroundColor = p.EstadoActivo ? ConsoleColor.White : ConsoleColor.DarkGray;
                        Console.WriteLine($"{p.Id.ToString().PadRight(6)}{p.Nombre.PadRight(25)}{(p.EstadoActivo ? "✅" : "❌")}");
                    }
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                MostrarError($"Error al listar pacientes: {ex.Message}");
            }

            EsperarContinuar();
        }
        #endregion

        #region Utilidades

        private void MostrarTitulo(string titulo)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(new string('═', titulo.Length + 6));
            Console.WriteLine($"== {titulo} ==");
            Console.WriteLine(new string('═', titulo.Length + 6));
            Console.ResetColor();
        }

        private void MostrarAdvertencia(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"⚠ {msg}");
            Console.ResetColor();
        }

        private void MostrarError(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ {msg}");
            Console.ResetColor();
        }

        private void MostrarExito(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        private bool ConfirmarAccion(string mensaje)
        {
            Console.Write($"\n{mensaje} (s/n): ");
            string r = Console.ReadLine()?.Trim().ToLower() ?? "";
            return r == "s";
        }

        private int LeerNumero(string mensaje)
        {
            Console.Write($"{mensaje}: ");
            if (!int.TryParse(Console.ReadLine(), out int num))
                return -1;
            return num;
        }

        private void EsperarContinuar()
        {
            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey(true);
        }

        #endregion
    }
}
