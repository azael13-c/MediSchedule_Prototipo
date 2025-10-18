using MediSchedule_Prototipo.Data;
using MediSchedule_Prototipo.Models;
using MediSchedule_Prototipo.Services;
using MediSchedule_Prototipo.Menus;
using MediSchedule_Prototipo.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;

namespace MediSchedule_Prototipo
{
    class Program
    {
        static void Main()
        {
            #region Configuración Inicial de Consola
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "MediSchedule Prototipo";

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=============================================================");
            Console.WriteLine("     🌿 MediSchedule — Tu Salud; Tu Tiempo; Tu Control 😉");
            Console.WriteLine("=============================================================");
            Console.ResetColor();
            #endregion

            #region Inicialización de Base de Datos y Servicios
            var options = new DbContextOptionsBuilder<MediScheduleContext>()
                .UseSqlite("Data Source=medischedule.db")
                .Options;

            using var context = new MediScheduleContext(options);
            context.Database.EnsureCreated();

            var usuarioService = new UsuarioService(context);
            var puestoService = new PuestoSaludService(context);
            var citaService = new CitaService(context);
            #endregion

            #region Creación Automática del SuperAdmin
            if (!context.Usuarios.Any(u => u.Rol == Rol.SuperAdmin))
            {
                Logger.Limpiar(); // 🧹 Reinicia historial si es una nueva instalación o recreación del superadmin

                var super = new Usuario("SuperAdmin", "1234", Rol.SuperAdmin);
                context.Usuarios.Add(super);
                context.SaveChanges();

                Logger.Registrar("Sistema", "Interno", "🧑‍💼 SuperAdmin creado/restaurado con ID: 1");
                Console.WriteLine("✅ SuperAdmin creado/restaurado con ID: 1 | Contraseña: 1234");
                Thread.Sleep(1000);
            }
            #endregion

            #region Proceso de Login
            Usuario? usuario = null;
            int MAX_INTENTOS = 3;
            int intentos = 0;

            while (usuario == null && intentos < MAX_INTENTOS)
            {
                Console.WriteLine("\n🔐 Inicie sesión");
                Console.Write("ID: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("⚠ Ingrese un número válido.");
                    intentos++;
                    continue;
                }

                Console.Write("Contraseña: ");
                string contrasena = Console.ReadLine() ?? "";

                try
                {
                    usuario = usuarioService.Login(id, contrasena);

                    if (usuario == null)
                    {
                        Console.WriteLine("❌ Credenciales incorrectas.");
                        intentos++;
                    }
                    else if (!usuario.EstadoActivo)
                    {
                        Console.WriteLine("⚠ Usuario inactivo. Contacte al administrador.");
                        usuario = null;
                        intentos++;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error: {ex.Message}");
                    intentos++;
                }

                if (usuario == null && intentos < MAX_INTENTOS)
                {
                    Console.WriteLine($"Intento {intentos}/{MAX_INTENTOS}...");
                    Thread.Sleep(1000);
                }
            }

            if (usuario == null)
            {
                Console.WriteLine("\n🚫 Demasiados intentos fallidos. Saliendo...");
                Thread.Sleep(1500);
                return;
            }
            #endregion

            #region Acceso y Menús Según Rol
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✅ Bienvenido, {usuario.Nombre} ({usuario.Rol})");
            Console.ResetColor();

            bool salirSesion = false;
            while (!salirSesion)
            {
                switch (usuario.Rol)
                {
                    case Rol.SuperAdmin:
                        salirSesion = new MenuSuperAdmin(usuarioService, puestoService, citaService).Mostrar();
                        break;
                    case Rol.Admin:
                        salirSesion = new MenuAdmin(usuarioService, puestoService, citaService).Mostrar(usuario);
                        break;
                    case Rol.PersonalMedico:
                        salirSesion = new MenuPersonalMedico(citaService, usuarioService).Mostrar(usuario);
                        break;
                    case Rol.Paciente:
                        salirSesion = new MenuPaciente(citaService, usuarioService).Mostrar(usuario);
                        break;
                }
            }
            #endregion

            #region Cierre de Sesión
            Console.WriteLine("\n👋 Sesión cerrada. Hasta luego!");
            Thread.Sleep(1000);
            #endregion
        }
    }
}
