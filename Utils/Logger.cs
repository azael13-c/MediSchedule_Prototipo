using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MediSchedule_Prototipo.Utils
{
    public static class Logger
    {
        private static readonly List<string> _historial = new();
        private static readonly string _rutaArchivo = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Data", "historial_acciones.txt"
        );
        private static bool _cargado = false;

        static Logger()
        {
            try
            {
                var dir = Path.GetDirectoryName(_rutaArchivo);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("⚠ Error inicializando Logger.");
                Console.ResetColor();
            }
        }

        // 🔹 Registrar acción del sistema interno
        public static void Registrar(string accion) => Registrar("Sistema", "Interno", accion);

        // 🔹 Registrar acción de un usuario real (SuperAdmin, Admin, etc.)
        public static void Registrar(string usuario, string rol, string accion)
        {
            string entrada = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | [{rol}] {usuario}: {accion}";
            _historial.Add(entrada);

            // Mantener el historial con un máximo de 200 líneas
            if (_historial.Count > 200)
                _historial.RemoveAt(0);

            try
            {
                File.AppendAllText(_rutaArchivo, entrada + Environment.NewLine);
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ Error al escribir en el log.");
                Console.ResetColor();
            }
        }

        // 🔹 Devuelve todo el historial
        public static IEnumerable<string> ObtenerHistorial()
        {
            CargarDesdeArchivo();
            return _historial.ToArray();
        }

        // 🔹 Limpia el historial (usado al reiniciar sistema o recrear SuperAdmin)
        public static void Limpiar()
        {
            _historial.Clear();
            try
            {
                if (File.Exists(_rutaArchivo))
                    File.Delete(_rutaArchivo);
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("⚠ No se pudo limpiar el historial.");
                Console.ResetColor();
            }
        }

        // 🔹 Carga el historial previo desde archivo
        public static void CargarDesdeArchivo()
        {
            if (_cargado) return;

            try
            {
                if (File.Exists(_rutaArchivo))
                {
                    var lineas = File.ReadAllLines(_rutaArchivo);
                    if (lineas.Length > 0)
                    {
                        _historial.Clear();
                        _historial.AddRange(lineas.TakeLast(200));
                    }
                }
                _cargado = true;
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("⚠ No se pudo cargar historial previo.");
                Console.ResetColor();
            }
        }

        // 🔹 Mostrar en consola con colores según tipo de usuario
        public static void MostrarEnConsola()
        {
            if (_historial.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n⚠ No hay acciones registradas todavía.");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n📜 HISTORIAL DE ACCIONES");
            Console.ResetColor();
            Console.WriteLine(new string('═', 70));

            foreach (var linea in _historial.TakeLast(100))
            {
                if (linea.Contains("[SuperAdmin]"))
                    Console.ForegroundColor = ConsoleColor.Magenta;
                else if (linea.Contains("[Admin]"))
                    Console.ForegroundColor = ConsoleColor.Yellow;
                else if (linea.Contains("[PersonalMedico]") || linea.Contains("[Médico]"))
                    Console.ForegroundColor = ConsoleColor.Cyan;
                else if (linea.Contains("[Paciente]"))
                    Console.ForegroundColor = ConsoleColor.White;
                else
                    Console.ResetColor();

                Console.WriteLine(linea);
                Console.ResetColor();
            }

            Console.WriteLine(new string('═', 70));
        }
    }
}
