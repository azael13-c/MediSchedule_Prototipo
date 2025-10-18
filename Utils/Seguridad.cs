using BCrypt.Net;

namespace MediSchedule_Prototipo.Utils
{
    public static class Seguridad
    {
        
        /// Genera un hash seguro de la contraseña usando BCrypt.
        
        /// <param name="contrasena">Contraseña en texto plano</param>
        /// <returns>Hash seguro</returns>
        public static string GenerarHash(string contrasena)
        {
            // WorkFactor = 12 es un buen equilibrio entre seguridad y rendimiento
            return BCrypt.Net.BCrypt.HashPassword(contrasena, workFactor: 12);
        }

        
        /// Verifica si la contraseña ingresada coincide con el hash almacenado.
      
        /// <param name="contrasena">Contraseña ingresada</param>
        /// <param name="hashAlmacenado">Hash almacenado en la DB</param>
        /// <returns>true si coinciden, false si no</returns>
        public static bool VerificarHash(string contrasena, string hashAlmacenado)
        {
            return BCrypt.Net.BCrypt.Verify(contrasena, hashAlmacenado);
        }
    }
}
