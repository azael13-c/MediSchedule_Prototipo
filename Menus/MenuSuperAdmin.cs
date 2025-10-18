using MediSchedule_Prototipo.Models;
using MediSchedule_Prototipo.Services;
using MediSchedule_Prototipo.Utils;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace MediSchedule_Prototipo.Menus
{
    public class MenuSuperAdmin
    {
        private readonly UsuarioService _usuarioService;
        private readonly PuestoSaludService _puestoService;
        private readonly CitaService _citaService;

        public MenuSuperAdmin(UsuarioService usuarioService, PuestoSaludService puestoService, CitaService citaService)
        {
            _usuarioService = usuarioService;
            _puestoService = puestoService;
            _citaService = citaService;

            // Cargar historial si existe
            Logger.CargarDesdeArchivo();
        }

        public bool Mostrar()
        {
            while (true)
            {
                Console.Clear();
                MostrarTitulo();

                // ==== MENÚ CATEGORIZADO ====
                Console.WriteLine("🩺 Puestos de Salud");
                Console.WriteLine("1️⃣  Crear puesto de salud");
                Console.WriteLine("2️⃣  Listar puestos de salud (filtros)");
                Console.WriteLine("3️⃣  Activar/Desactivar puesto\n");

                Console.WriteLine("👤 Usuarios");
                Console.WriteLine("4️⃣  Crear usuario (Admin, Médico, Paciente)");
                Console.WriteLine("5️⃣  Listar usuarios (filtros)");
                Console.WriteLine("6️⃣  Activar/Desactivar usuario\n");

                Console.WriteLine("📅 Citas y Historial");
                Console.WriteLine("7️⃣  Ver todas las citas");
                Console.WriteLine("8️⃣  Ver historial de acciones\n");

                Console.WriteLine("0️⃣  Cerrar sesión");
                Console.Write("→ Opción: ");
                string opcion = Console.ReadLine()?.Trim() ?? "";

                try
                {
                    switch (opcion)
                    {
                        case "1": CrearPuesto(); break;
                        case "2": ListarPuestosConFiltros(); break;
                        case "3": ActivarDesactivarPuesto(); break;
                        case "4": CrearUsuario(); break;
                        case "5": ListarUsuariosConFiltros(); break;
                        case "6": ActivarDesactivarUsuario(); break;
                        case "7": VerCitas(); break;
                        case "8": VerHistorialAcciones(); break;
                        case "0": return true;
                        default: MostrarAdvertencia("Opción inválida."); break;
                    }
                }
                catch (Exception ex)
                {
                    MostrarError($"Error inesperado: {ex.Message}");
                }
            }
        }

        #region UI Helpers

        private void MostrarTitulo()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=============================================================");
            Console.WriteLine($"🧑‍💼 MENÚ SUPERADMIN      {DateTime.Now:yyyy-MM-dd HH:mm}");
            Console.WriteLine("=============================================================");
            Console.ResetColor();
        }

        private void MostrarExito(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✅ {mensaje}");
            Console.ResetColor();
            Console.WriteLine("Presiona cualquier tecla para volver al menú...");
            Console.ReadKey(true);
        }

        private void MostrarAdvertencia(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"⚠ {mensaje}");
            Console.ResetColor();
            Console.WriteLine("Presiona cualquier tecla para volver al menú...");
            Console.ReadKey(true);
        }

        private void MostrarError(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ {mensaje}");
            Console.ResetColor();
            Console.WriteLine("Presiona cualquier tecla para volver al menú...");
            Console.ReadKey(true);
        }

        private bool ConfirmarAccion(string mensaje)
        {
            Console.Write($"{mensaje} (s/n): ");
            string resp = Console.ReadLine()?.Trim().ToLower() ?? "n";
            return resp == "s";
        }

        #endregion

        #region Puestos

        private void CrearPuesto()
        {
            Console.Clear();
            MostrarTitulo();

            string nombre;
            while (true)
            {
                Console.Write("Nombre del puesto (mínimo 3 caracteres, no numérico): ");
                nombre = Console.ReadLine()?.Trim() ?? "";

                if (string.IsNullOrWhiteSpace(nombre) || nombre.Length < 3)
                {
                    MostrarAdvertencia("El nombre debe tener al menos 3 caracteres.");
                    continue;
                }

                if (int.TryParse(nombre, out _))
                {
                    MostrarAdvertencia("El nombre no puede ser solo un número.");
                    continue;
                }

                if (_puestoService.ListarTodos().AsEnumerable()
                    .Any(p => string.Equals(p.Nombre, nombre, StringComparison.OrdinalIgnoreCase)))
                {
                    MostrarAdvertencia("Ya existe un puesto con ese nombre.");
                    continue;
                }

                break;
            }

            try
            {
                var creado = _puestoService.CrearPuesto(nombre);
                Logger.Registrar($"🧱 Creó el puesto de salud '{creado.Nombre}' (ID: {creado.Id})");
                MostrarExito($"Puesto creado con ID: {creado.Id}");
            }
            catch (Exception ex)
            {
                string msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MostrarError($"❌ Error al crear puesto: {msg}");
            }
        }

        private void ListarPuestosConFiltros()
        {
            Console.WriteLine("\nFiltrar puestos:");
            Console.WriteLine("1 - Todos");
            Console.WriteLine("2 - Activos");
            Console.WriteLine("3 - Inactivos");
            Console.WriteLine("4 - Buscar por nombre");
            Console.Write("→ Elección: ");
            string choice = Console.ReadLine()?.Trim() ?? "1";

            var query = _puestoService.ListarTodos().AsEnumerable();

            switch (choice)
            {
                case "2": query = query.Where(p => p.EstadoActivo); break;
                case "3": query = query.Where(p => !p.EstadoActivo); break;
                case "4":
                    Console.Write("Texto a buscar (nombre): ");
                    string q = Console.ReadLine()?.Trim() ?? "";
                    if (string.IsNullOrWhiteSpace(q))
                    {
                        MostrarAdvertencia("Texto de búsqueda vacío.");
                        return;
                    }
                    query = query.Where(p => p.Nombre.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0);
                    break;
                default: break;
            }

            var list = query.OrderBy(p => p.Id).ToList();
            if (!list.Any())
            {
                MostrarAdvertencia("No se encontraron puestos con ese criterio.");
                return;
            }

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\n📋 Lista de Puestos:");
            Console.ResetColor();
            Console.WriteLine("ID".PadRight(6) + "Nombre".PadRight(30) + "Activo");
            Console.WriteLine(new string('-', 50));
            foreach (var p in list)
            {
                Console.ForegroundColor = p.EstadoActivo ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine($"{p.Id.ToString().PadRight(6)}{p.Nombre.PadRight(30)}{p.EstadoActivo}");
                Console.ResetColor();
            }
            Console.WriteLine("\nPresiona cualquier tecla para volver al menú...");
            Console.ReadKey(true);
        }

        private void ActivarDesactivarPuesto()
        {
            Console.Clear();
            MostrarTitulo();

            var puestos = _puestoService.ListarTodos().ToList();
            if (!puestos.Any())
            {
                MostrarAdvertencia("No hay puestos registrados en el sistema.");
                return;
            }

            Console.WriteLine("Puestos disponibles:\n");
            Console.WriteLine("ID | Nombre                          | Estado");
            Console.WriteLine(new string('-', 50));
            foreach (var p in puestos)
            {
                Console.ForegroundColor = p.EstadoActivo ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine($"{p.Id,-3} | {p.Nombre,-30} | {(p.EstadoActivo ? "Activo" : "Inactivo")}");
                Console.ResetColor();
            }
            Console.WriteLine(new string('-', 50));

            Console.Write("\nIngrese el ID del puesto que desea modificar: ");
            if (!int.TryParse(Console.ReadLine(), out int idP))
            {
                MostrarAdvertencia("ID inválido.");
                return;
            }

            var puesto = _puestoService.ObtenerPorId(idP);
            if (puesto == null)
            {
                MostrarAdvertencia("Puesto no encontrado.");
                return;
            }

            Console.WriteLine($"\nPuesto: {puesto.Nombre} | Estado actual: {(puesto.EstadoActivo ? "Activo" : "Inactivo")}");
            Console.Write("¿Qué desea hacer? (a = Activar / d = Desactivar): ");
            string acc = Console.ReadLine()?.ToLower() ?? "";

            if (acc != "a" && acc != "d")
            {
                MostrarAdvertencia("Opción inválida.");
                return;
            }

            if (!ConfirmarAccion("¿Está seguro que desea continuar?"))
            {
                MostrarAdvertencia("Acción cancelada.");
                return;
            }

            try
            {
                if (acc == "a")
                {
                    _puestoService.ActivarPuesto(idP);
                    Logger.Registrar($"✅ Activó el puesto '{puesto.Nombre}' (ID: {puesto.Id})");
                }
                else
                {
                    _puestoService.DesactivarPuesto(idP);
                    Logger.Registrar($"🚫 Desactivó el puesto '{puesto.Nombre}' (ID: {puesto.Id})");
                }

                MostrarExito("✅ Acción completada correctamente.");
            }
            catch (Exception ex)
            {
                string msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MostrarError($"❌ Error al modificar puesto: {msg}");
            }
        }
        #endregion
        #region Usuarios

        private void CrearUsuario()
        {
            Console.Clear();
            MostrarTitulo();

            // --- Nombre ---
            string nomU;
            while (true)
            {
                Console.Write("Nombre (mínimo 3 caracteres): ");
                nomU = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(nomU) || nomU.Length < 3)
                {
                    MostrarAdvertencia("El nombre debe tener al menos 3 caracteres.");
                    continue;
                }

                if (_usuarioService.ListarTodos().AsEnumerable()
                    .Any(u => u.Nombre.Equals(nomU, StringComparison.OrdinalIgnoreCase)))
                {
                    MostrarAdvertencia("Ya existe un usuario con ese nombre.");
                    continue;
                }
                break;
            }

            // --- Contraseña ---
            string passU;
            while (true)
            {
                Console.Write("Contraseña (mínimo 6 caracteres, letras+números): ");
                passU = (Console.ReadLine() ?? "").Trim();
                bool tieneLetra = passU.Any(char.IsLetter);
                bool tieneNumero = passU.Any(char.IsDigit);
                if (tieneLetra && tieneNumero && passU.Length >= 6) break;
                MostrarAdvertencia("Contraseña inválida. Debe tener 6+ caracteres y al menos una letra y un número.");
            }

            // --- Rol ---
            int rolNum;
            while (true)
            {
                Console.Write("Rol (1=Admin, 2=Médico, 3=Paciente): ");
                if (int.TryParse(Console.ReadLine(), out rolNum) && rolNum >= 1 && rolNum <= 3)
                    break;
                MostrarAdvertencia("Rol inválido. Ingrese 1, 2 o 3.");
            }

            Rol rol = rolNum switch
            {
                1 => Rol.Admin,
                2 => Rol.PersonalMedico,
                3 => Rol.Paciente,
                _ => Rol.Paciente
            };

            // --- Selección de puesto ---
            int? pid = null;
            var puestosActivos = _puestoService.ListarTodos().Where(p => p.EstadoActivo).ToList();
            if (!puestosActivos.Any())
            {
                MostrarAdvertencia("No hay puestos activos disponibles. Crea primero un puesto.");
                return;
            }

            Console.WriteLine("\nPuestos activos disponibles:");
            foreach (var p in puestosActivos)
                Console.WriteLine($"{p.Id} - {p.Nombre}");

            while (pid == null)
            {
                Console.Write("ID Puesto (obligatorio): ");
                string? pidInput = Console.ReadLine();
                if (int.TryParse(pidInput, out int pidVal) && puestosActivos.Any(p => p.Id == pidVal))
                {
                    pid = pidVal;
                    break;
                }
                MostrarAdvertencia("ID inválido o puesto no activo. Intenta de nuevo.");
            }

            // --- Crear usuario ---
            try
            {
                var nuevoUsuario = _usuarioService.CrearUsuario(nomU, passU, rol, pid);
                Logger.Registrar($"👤 Creó un nuevo usuario '{nuevoUsuario.Nombre}' (Rol: {nuevoUsuario.Rol}, ID: {nuevoUsuario.Id})");
                MostrarExito($"✅ Usuario creado exitosamente.\n  ID: {nuevoUsuario.Id}\n  Rol: {nuevoUsuario.Rol}\n  Email generado: {nuevoUsuario.Email}");
            }
            catch (Exception ex)
            {
                string msg = ex.InnerException?.Message ?? ex.Message;
                MostrarError($"❌ Error al crear usuario: {msg}");
            }
        }

        private void ListarUsuariosConFiltros()
        {
            int intentos = 0;
            string choice = "";

            while (intentos < 2)
            {
                Console.WriteLine("\nFiltrar usuarios:");
                Console.WriteLine("1 - Todos");
                Console.WriteLine("2 - Activos");
                Console.WriteLine("3 - Inactivos");
                Console.WriteLine("4 - Buscar por nombre");
                Console.WriteLine("5 - Filtrar por rol");
                Console.Write("→ Elección: ");
                choice = Console.ReadLine()?.Trim() ?? "";

                if (new[] { "1", "2", "3", "4", "5" }.Contains(choice))
                    break;

                intentos++;
                MostrarAdvertencia("❌ Opción inválida. Inténtalo nuevamente.");
            }

            if (intentos >= 2)
            {
                MostrarAdvertencia("Demasiados intentos inválidos. Regresando al menú principal...");
                return;
            }

            var query = _usuarioService.ListarTodos().AsEnumerable();

            switch (choice)
            {
                case "2": query = query.Where(u => u.EstadoActivo); break;
                case "3": query = query.Where(u => !u.EstadoActivo); break;
                case "4":
                    Console.Write("Texto a buscar (nombre): ");
                    string q = Console.ReadLine()?.Trim() ?? "";
                    if (string.IsNullOrWhiteSpace(q)) { MostrarAdvertencia("Texto vacío."); return; }
                    query = query.Where(u => u.Nombre.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0);
                    break;
                case "5":
                    Console.WriteLine("Roles: 1=Admin, 2=Médico, 3=Paciente, 4=SuperAdmin");
                    Console.Write("Ingrese número de rol: ");
                    if (!int.TryParse(Console.ReadLine(), out int rNum)) { MostrarAdvertencia("Rol inválido."); return; }
                    var r = rNum switch { 1 => Rol.Admin, 2 => Rol.PersonalMedico, 3 => Rol.Paciente, 4 => Rol.SuperAdmin, _ => (Rol?)null };
                    if (!r.HasValue) { MostrarAdvertencia("Rol inválido."); return; }
                    query = query.Where(u => u.Rol == r.Value);
                    break;
                default: break;
            }

            var list = query.OrderBy(u => u.Id).ToList();
            if (!list.Any()) { MostrarAdvertencia("No se encontraron usuarios con ese criterio."); return; }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n👥 Lista de Usuarios:");
            Console.ResetColor();
            Console.WriteLine("ID".PadRight(6) + "Nombre".PadRight(25) + "Rol".PadRight(15) + "Activo");
            Console.WriteLine(new string('-', 60));

            foreach (var u in list)
            {
                Console.ForegroundColor = u.EstadoActivo ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine($"{u.Id.ToString().PadRight(6)}{u.Nombre.PadRight(25)}{u.Rol.ToString().PadRight(15)}{u.EstadoActivo}");
                Console.ResetColor();
            }

            Console.WriteLine("\nPresiona cualquier tecla para volver al menú...");
            Console.ReadKey(true);
        }

        private void ActivarDesactivarUsuario()
        {
            var lista = _usuarioService.ListarTodos().ToList();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n👥 Usuarios disponibles:");
            Console.ResetColor();
            Console.WriteLine("ID".PadRight(6) + "Nombre".PadRight(25) + "Rol".PadRight(15) + "Activo");
            Console.WriteLine(new string('-', 60));
            foreach (var u in lista)
            {
                Console.ForegroundColor = u.EstadoActivo ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine($"{u.Id.ToString().PadRight(6)}{u.Nombre.PadRight(25)}{u.Rol.ToString().PadRight(15)}{u.EstadoActivo}");
                Console.ResetColor();
            }

            Console.Write("\nID usuario: ");
            if (!int.TryParse(Console.ReadLine(), out int idU))
            {
                MostrarAdvertencia("ID inválido.");
                return;
            }

            var usuario = _usuarioService.ObtenerPorId(idU);
            if (usuario == null)
            {
                MostrarAdvertencia("Usuario no encontrado.");
                return;
            }

            if (usuario.Rol == Rol.SuperAdmin)
            {
                MostrarAdvertencia("⚠️ El SuperAdmin no puede ser activado ni desactivado.");
                return;
            }

            Console.WriteLine($"Usuario: {usuario.Nombre} | Rol: {usuario.Rol} | Activo: {usuario.EstadoActivo}");
            Console.Write("¿Activar o desactivar? (a/d): ");
            string accU = Console.ReadLine()?.ToLower() ?? "";

            if (accU != "a" && accU != "d")
            {
                MostrarAdvertencia("Opción inválida.");
                return;
            }

            if (!ConfirmarAccion("¿Estás seguro que deseas continuar?"))
            {
                MostrarAdvertencia("Acción cancelada.");
                return;
            }

            try
            {
                if (accU == "a")
                {
                    _usuarioService.ActivarUsuario(idU);
                    Logger.Registrar($"🔓 Activó al usuario '{usuario.Nombre}' (Rol: {usuario.Rol}, ID: {usuario.Id})");
                }
                else
                {
                    _usuarioService.DesactivarUsuario(idU);
                    Logger.Registrar($"🔒 Desactivó al usuario '{usuario.Nombre}' (Rol: {usuario.Rol}, ID: {usuario.Id})");
                }

                MostrarExito("Acción completada.");
            }
            catch (Exception ex)
            {
                MostrarError($"Error al modificar usuario: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        #endregion

        #region Citas

        private void VerCitas()
        {
            var citas = _citaService.ObtenerTodas().OrderBy(c => c.Fecha).ToList();
            if (!citas.Any())
            {
                MostrarAdvertencia("No hay citas registradas.");
                return;
            }

            Logger.Registrar($"📅 Consultó todas las citas ({citas.Count} registradas)");

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n📅 Todas las citas (ordenadas por fecha):");
            Console.ResetColor();
            Console.WriteLine("ID".PadRight(6) + "Paciente".PadRight(24) + "Médico".PadRight(24) + "Fecha/Hora");
            Console.WriteLine(new string('-', 90));

            foreach (var c in citas)
            {
                string pacNombre = c.Paciente?.Nombre ?? "—";
                string medNombre = c.PersonalMedico?.Nombre ?? "—";
                string fechaStr = c.Fecha.ToString("yyyy-MM-dd HH:mm");
                Console.WriteLine($"{c.Id.ToString().PadRight(6)}{pacNombre.PadRight(24)}{medNombre.PadRight(24)}{fechaStr}");
            }

            Console.WriteLine("\nPresiona cualquier tecla para volver al menú...");
            Console.ReadKey(true);
        }

        #endregion

        #region Historial

        private void VerHistorialAcciones()
        {
            var registros = Logger.ObtenerHistorial().ToList();

            if (!registros.Any())
            {
                MostrarAdvertencia("No hay acciones registradas.");
                return;
            }

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("\n📜 HISTORIAL DE ACCIONES");
            Console.ResetColor();
            Console.WriteLine(new string('-', 70));

            foreach (var linea in registros.TakeLast(50))
            {
                if (linea.Contains("Creó")) Console.ForegroundColor = ConsoleColor.Green;
                else if (linea.Contains("Activó")) Console.ForegroundColor = ConsoleColor.Cyan;
                else if (linea.Contains("Desactivó")) Console.ForegroundColor = ConsoleColor.Red;
                else Console.ForegroundColor = ConsoleColor.White;

                Console.WriteLine(linea);
            }

            Console.ResetColor();
            Console.WriteLine("\nPresiona cualquier tecla para volver al menú...");
            Console.ReadKey(true);
        }

        #endregion
    }
}
