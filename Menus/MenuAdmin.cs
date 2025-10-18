using MediSchedule_Prototipo.Models;
using MediSchedule_Prototipo.Services;
using MediSchedule_Prototipo.Utils;
using System;
using System.Linq;
using System.Threading;

namespace MediSchedule_Prototipo.Menus
{
    public class MenuAdmin
    {
        private readonly UsuarioService _usuarioService;
        private readonly PuestoSaludService _puestoService;
        private readonly CitaService _citaService;

        public MenuAdmin(UsuarioService usuarioService, PuestoSaludService puestoService, CitaService citaService)
        {
            _usuarioService = usuarioService;
            _puestoService = puestoService;
            _citaService = citaService;
            Logger.CargarDesdeArchivo();
        }

        public bool Mostrar(Usuario admin)
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"=============================================================");
                Console.WriteLine($"🏢 MENÚ ADMINISTRADOR | {admin.Nombre} ({admin.Rol})");
                Console.WriteLine($"=============================================================");
                Console.ResetColor();

                Console.WriteLine("1️⃣  Crear personal médico");
                Console.WriteLine("2️⃣  Crear paciente");
                Console.WriteLine("3️⃣  Listar usuarios de mi puesto");
                Console.WriteLine("4️⃣  Activar/Desactivar usuario");
                Console.WriteLine("5️⃣  Crear cita");
                Console.WriteLine("6️⃣  Listar citas de mi puesto");
                Console.WriteLine("0️⃣  Cerrar sesión");
                Console.WriteLine("-------------------------------------------------------------");
                Console.Write("→ Opción: ");
                string op = Console.ReadLine()?.Trim() ?? "";

                try
                {
                    switch (op)
                    {
                        case "1":
                        case "2":
                            CrearUsuario(op, admin);
                            break;
                        case "3":
                            ListarUsuariosPorPuesto(admin.PuestoSaludId ?? 0);
                            break;
                        case "4":
                            ActivarDesactivarUsuario(admin);
                            break;
                        case "5":
                            CrearCita(admin);
                            break;
                        case "6":
                            ListarCitasPorPuesto(admin.PuestoSaludId ?? 0);
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
        // Creacion de usuarios (P.médico o paciente)

        private void CrearUsuario(string opcion, Usuario admin)
        {
            Console.Clear();
            MostrarTitulo(opcion == "1" ? "CREAR PERSONAL MÉDICO" : "CREAR PACIENTE");

            try
            {
                // Leer nombre del usuario
                string nombre = LeerTexto("Nombre", min: 3);
                if (nombre == null) return;

                // Leer contraseña
                string pass = LeerTexto("Contraseña", min: 4);
                if (pass == null) return;

                //  Determinar el rol según la opción
                Rol rol = opcion == "1" ? Rol.PersonalMedico : Rol.Paciente;

                // Confirmar creación
                Console.WriteLine($"\n⚙️  Vas a crear un usuario:");
                Console.WriteLine($"   👤 Nombre: {nombre}");
                Console.WriteLine($"   🎭 Rol: {rol}");
                if (admin.PuestoSaludId.HasValue)
                    Console.WriteLine($"   🏥 Puesto Salud ID (heredado del admin): {admin.PuestoSaludId}");

                if (!ConfirmarAccion("\n¿Deseas continuar con la creación? (s/n): "))
                {
                    MostrarAdvertencia("Creación cancelada por el usuario.");
                    return;
                }

                // Crear el usuario
                var nuevo = _usuarioService.CrearUsuario(nombre, pass, rol, admin.PuestoSaludId ?? 0);

                // Mostrar resultado
                MostrarExito($"✅ Usuario '{nuevo.Nombre}' creado exitosamente con ID: {nuevo.Id}");

                // Registrar acción en el Logger
                Logger.Registrar(
                    admin.Nombre,
                    admin.Rol.ToString(),
                    $"👤 Creó usuario '{nuevo.Nombre}' (Rol: {nuevo.Rol}, ID: {nuevo.Id})"
                );
            }
            catch (ArgumentException ex)
            {
                MostrarAdvertencia($"⚠️ Error de validación: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                MostrarAdvertencia($"🚫 Operación no permitida: {ex.Message}");
            }
            catch (Exception ex)
            {
                string msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MostrarError($"❌ Error inesperado al crear el usuario: {msg}");
            }
            finally
            {
                Console.WriteLine("\nPresiona cualquier tecla para volver al menú...");
                Console.ReadKey(true);
            }
        }


        #region Listar Usuarios (Filtrado)
        private void ListarUsuariosPorPuesto(int puestoId)
        {
            Console.Clear();
            MostrarTitulo("👥 LISTAR USUARIOS DE MI PUESTO");

            var usuarios = _usuarioService.ListarPorPuesto(puestoId).ToList();

            if (!usuarios.Any())
            {
                MostrarAdvertencia("⚠ No hay usuarios registrados en este puesto de salud.");
                return;
            }

            // 1️⃣ Seleccionar filtro por rol
            Console.WriteLine("Filtrar por tipo de usuario:");
            Console.WriteLine("1️⃣ Personal Médico");
            Console.WriteLine("2️⃣ Pacientes");
            Console.WriteLine("3️⃣ Todos");
            Console.Write("→ Opción: ");
            string filtroRol = Console.ReadLine()?.Trim() ?? "3";

            IEnumerable<Usuario> filtrados = usuarios;

            switch (filtroRol)
            {
                case "1":
                    filtrados = usuarios.Where(u => u.Rol == Rol.PersonalMedico);
                    break;
                case "2":
                    filtrados = usuarios.Where(u => u.Rol == Rol.Paciente);
                    break;
                default:
                    filtrados = usuarios;
                    break;
            }

            // 2️⃣ Filtro por estado
            Console.WriteLine("\nFiltrar por estado:");
            Console.WriteLine("1️⃣ Activos");
            Console.WriteLine("2️⃣ Inactivos");
            Console.WriteLine("3️⃣ Todos");
            Console.Write("→ Opción: ");
            string filtroEstado = Console.ReadLine()?.Trim() ?? "3";

            switch (filtroEstado)
            {
                case "1":
                    filtrados = filtrados.Where(u => u.EstadoActivo);
                    break;
                case "2":
                    filtrados = filtrados.Where(u => !u.EstadoActivo);
                    break;
                default:
                    break;
            }

            filtrados = filtrados.ToList();

            // 3️⃣ Resultado
            Console.Clear();
            MostrarTitulo($"RESULTADOS ({filtrados.Count()} usuarios encontrados)");

            if (!filtrados.Any())
            {
                MostrarAdvertencia("⚠ No hay usuarios que coincidan con los filtros seleccionados.");
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("ID   | NOMBRE            | ROL              | ESTADO");
            Console.ResetColor();
            Console.WriteLine(new string('-', 55));

            foreach (var u in filtrados)
            {
                Console.ForegroundColor = u.EstadoActivo ? ConsoleColor.White : ConsoleColor.DarkGray;
                string estado = u.EstadoActivo ? "Activo ✅" : "Inactivo ❌";
                Console.WriteLine($"{u.Id,-4} | {u.Nombre,-17} | {u.Rol,-16} | {estado}");
            }

            Console.ResetColor();
            Console.WriteLine("\nPresione una tecla para regresar al menú...");
            Console.ReadKey(true);
        }
        #endregion



        #region Activar / Desactivar Usuario
        private void ActivarDesactivarUsuario(Usuario admin)
        {
            Console.Clear();
            MostrarTitulo("🔒 ACTIVAR / DESACTIVAR USUARIO");

            try
            {
                // 1️⃣ Mostrar usuarios disponibles (solo del mismo puesto)
                var usuarios = _usuarioService.ListarPorPuesto(admin.PuestoSaludId ?? 0).ToList();
                if (!usuarios.Any())
                {
                    MostrarAdvertencia("No hay usuarios registrados en su puesto.");
                    return;
                }

                Console.WriteLine("👥 Usuarios disponibles:");
                foreach (var u in usuarios)
                {
                    string estado = u.EstadoActivo ? "Activo ✅" : "Inactivo ❌";
                    Console.WriteLine($"   ID: {u.Id,-3} | {u.Nombre,-20} | {u.Rol,-15} | {estado}");
                }

                // 2️⃣ Leer ID
                int idU = LeerNumero("\nIngrese el ID del usuario a modificar");
                if (idU == -1) return;

                var usuario = _usuarioService.ObtenerPorId(idU);
                if (usuario == null)
                {
                    MostrarAdvertencia("⚠ Usuario no encontrado.");
                    return;
                }

                // 3️⃣ Evitar que el Admin se desactive a sí mismo o al SuperAdmin
                if (usuario.Id == admin.Id)
                {
                    MostrarAdvertencia("No puede desactivarse a sí mismo.");
                    return;
                }
                if (usuario.Rol == Rol.SuperAdmin)
                {
                    MostrarAdvertencia("No tiene permisos para desactivar al SuperAdmin.");
                    return;
                }

                Console.WriteLine($"\nUsuario seleccionado:");
                Console.WriteLine($"👤 {usuario.Nombre} | Rol: {usuario.Rol} | Estado actual: {(usuario.EstadoActivo ? "Activo ✅" : "Inactivo ❌")}");

                // 4️⃣ Seleccionar acción
                string accion;
                do
                {
                    Console.Write("¿Desea (a)ctivar o (d)esactivar?: ");
                    accion = Console.ReadLine()?.Trim().ToLower() ?? "";
                    if (accion != "a" && accion != "d")
                    {
                        MostrarAdvertencia("Opción inválida. Ingrese 'a' o 'd'.");
                    }
                } while (accion != "a" && accion != "d");

                // 5️⃣ Confirmación de seguridad
                if (!ConfirmarAccion($"¿Está seguro de {(accion == "a" ? "activar" : "desactivar")} al usuario '{usuario.Nombre}'?"))
                {
                    MostrarAdvertencia("Operación cancelada por el usuario.");
                    return;
                }

                // 6️⃣ Aplicar acción
                if (accion == "a")
                {
                    if (usuario.EstadoActivo)
                    {
                        MostrarAdvertencia($"El usuario '{usuario.Nombre}' ya está activo.");
                        return;
                    }

                    _usuarioService.ActivarUsuario(idU);
                    MostrarExito($"✅ Usuario '{usuario.Nombre}' activado correctamente.");
                    Logger.Registrar(admin.Nombre, admin.Rol.ToString(), $"✅ Activó usuario '{usuario.Nombre}' (Rol: {usuario.Rol}, ID: {usuario.Id})");
                }
                else
                {
                    if (!usuario.EstadoActivo)
                    {
                        MostrarAdvertencia($"El usuario '{usuario.Nombre}' ya está inactivo.");
                        return;
                    }

                    _usuarioService.DesactivarUsuario(idU);
                    MostrarAdvertencia($"🔒 Usuario '{usuario.Nombre}' desactivado correctamente.");
                    Logger.Registrar(admin.Nombre, admin.Rol.ToString(), $"🔒 Desactivó usuario '{usuario.Nombre}' (Rol: {usuario.Rol}, ID: {usuario.Id})");
                }
            }
            catch (ArgumentException ex)
            {
                MostrarAdvertencia($"⚠ Error de validación: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                MostrarAdvertencia($"🚫 Operación no permitida: {ex.Message}");
            }
            catch (Exception ex)
            {
                string msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MostrarError($"❌ Error inesperado al cambiar estado: {msg}");
            }
            finally
            {
                Console.WriteLine("\nPresione cualquier tecla para volver al menú...");
                Console.ReadKey(true);
            }
        }
        #endregion


        #region Crear Cita
        private void CrearCita(Usuario admin)
        {
            Console.Clear();
            MostrarTitulo("CREAR CITA MÉDICA");

            try
            {
                // 1️⃣ Mostrar pacientes activos
                var pacientes = _usuarioService.ListarPorRol(Rol.Paciente)
                    .Where(p => p.EstadoActivo)
                    .ToList();

                if (!pacientes.Any())
                {
                    MostrarAdvertencia("No hay pacientes activos disponibles para asignar.");
                    return;
                }

                Console.WriteLine("👥 Pacientes disponibles:");
                foreach (var p in pacientes)
                    Console.WriteLine($"   ID: {p.Id,-3} | Nombre: {p.Nombre}");

                int pacId = LeerNumero("Seleccione ID del paciente");
                var paciente = pacientes.FirstOrDefault(p => p.Id == pacId);
                if (paciente == null)
                {
                    MostrarAdvertencia("Paciente no encontrado o inactivo.");
                    return;
                }

                Console.Clear();
                MostrarTitulo("CREAR CITA MÉDICA");

                // 2️⃣ Mostrar médicos activos
                var medicos = _usuarioService.ListarPorRol(Rol.PersonalMedico)
                    .Where(m => m.EstadoActivo)
                    .ToList();

                if (!medicos.Any())
                {
                    MostrarAdvertencia("No hay personal médico activo disponible.");
                    return;
                }

                Console.WriteLine("🩺 Médicos disponibles:");
                foreach (var m in medicos)
                    Console.WriteLine($"   ID: {m.Id,-3} | Nombre: {m.Nombre}");

                int medId = LeerNumero("Seleccione ID del médico");
                var medico = medicos.FirstOrDefault(m => m.Id == medId);
                if (medico == null)
                {
                    MostrarAdvertencia("Médico no encontrado o inactivo.");
                    return;
                }

                // 3️⃣ Leer fecha
                Console.Write("\n📅 Fecha (YYYY-MM-DD HH:MM): ");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime fecha))
                {
                    MostrarError("Formato de fecha inválido. Ejemplo: 2025-10-14 15:30");
                    return;
                }

                if (fecha < DateTime.Now)
                {
                    MostrarAdvertencia("La fecha no puede ser en el pasado.");
                    return;
                }

                // 4️⃣ Observaciones opcionales
                Console.Write("📝 Observaciones: ");
                string obs = Console.ReadLine()?.Trim() ?? "";

                // 5️⃣ Confirmar datos antes de crear
                Console.WriteLine("\n📋 Resumen de la cita:");
                Console.WriteLine($"   👤 Paciente: {paciente.Nombre} (ID: {paciente.Id})");
                Console.WriteLine($"   🩺 Médico: {medico.Nombre} (ID: {medico.Id})");
                Console.WriteLine($"   📅 Fecha: {fecha}");
                Console.WriteLine($"   📝 Observaciones: {(string.IsNullOrEmpty(obs) ? "(Ninguna)" : obs)}");

                if (!ConfirmarAccion("\n¿Deseas crear esta cita? (s/n): "))
                {
                    MostrarAdvertencia("Creación de cita cancelada.");
                    return;
                }

                // 6️⃣ Crear cita
                var cita = _citaService.CrearCita(admin, pacId, medId, fecha, obs);
                MostrarExito($"✅ Cita creada exitosamente con ID: {cita.Id}");

                // 7️⃣ Registro en Logger
                Logger.Registrar(
                    admin.Nombre,
                    admin.Rol.ToString(),
                    $"📅 Creó cita (ID: {cita.Id}) entre paciente '{paciente.Nombre}' (ID:{paciente.Id}) y médico '{medico.Nombre}' (ID:{medico.Id}) para {fecha:dd/MM/yyyy HH:mm}"
                );
            }
            catch (ArgumentException ex)
            {
                MostrarAdvertencia($"⚠️ Error de validación: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                MostrarAdvertencia($"🚫 Operación no permitida: {ex.Message}");
            }
            catch (Exception ex)
            {
                string msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MostrarError($"❌ Error inesperado al crear la cita: {msg}");
            }
            finally
            {
                Console.WriteLine("\nPresiona cualquier tecla para volver al menú...");
                Console.ReadKey(true);
            }
        }
        #endregion



        #region Listar Citas
        private void ListarCitasPorPuesto(int puestoId)
        {
            Console.Clear();
            MostrarTitulo("CITAS DE MI PUESTO");

            var citas = _citaService.ObtenerCitasPorPuesto(puestoId).ToList();
            if (!citas.Any())
            {
                MostrarAdvertencia("No hay citas registradas en este puesto.");
                return;
            }

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"📅 Total citas: {citas.Count}\n");
            Console.ResetColor();

            foreach (var c in citas)
            {
                Console.WriteLine($"ID:{c.Id,-3} | Paciente:{c.Paciente.Nombre,-10} | Médico:{c.PersonalMedico.Nombre,-10} | {c.Fecha:g}");
            }

            Console.WriteLine("\nPresione una tecla para volver...");
            Console.ReadKey(true);
        }
        #endregion

        #region Utilidades
        private void MostrarTitulo(string titulo)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"=============================================================");
            Console.WriteLine($"📋 {titulo}");
            Console.WriteLine($"=============================================================");
            Console.ResetColor();
        }

        private string LeerTexto(string campo, int min = 1)
        {
            int intentos = 0;
            while (intentos < 3)
            {
                Console.Write($"{campo}: ");
                string valor = Console.ReadLine()?.Trim() ?? "";
                if (valor.Length >= min) return valor;
                MostrarAdvertencia($"El campo '{campo}' requiere al menos {min} caracteres.");
                intentos++;
            }
            MostrarError($"Demasiados intentos. Operación cancelada.");
            return null;
        }

        private int LeerNumero(string mensaje)
        {
            int intentos = 0;
            while (intentos < 3)
            {
                Console.Write($"{mensaje}: ");
                if (int.TryParse(Console.ReadLine(), out int num))
                    return num;

                MostrarAdvertencia("Debe ingresar un número válido.");
                intentos++;
            }
            MostrarError("Demasiados intentos. Operación cancelada.");
            return -1;
        }

        private bool ConfirmarAccion(string mensaje)
        {
            Console.Write($"{mensaje} (s/n): ");
            string resp = Console.ReadLine()?.Trim().ToLower() ?? "n";
            return resp == "s";
        }

        private void MostrarExito(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✅ {msg}");
            Console.ResetColor();
            Thread.Sleep(1000);
        }

        private void MostrarAdvertencia(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"⚠ {msg}");
            Console.ResetColor();
            Thread.Sleep(1000);
        }

        private void MostrarError(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ {msg}");
            Console.ResetColor();
            Thread.Sleep(1500);
        }
        #endregion
    }
}
