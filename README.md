# MediSchedule_Prototipo
Prototipo en C# de **gestión de citas médicas** con **flujo multirol** en consola 
---

## 📦 Instalación / Preparación

1. **Clonar el repositorio:**
```bash
git clone https://github.com/azael13-c/MediSchedule_Prototipo.git
cd MediSchedule_Prototipo
O descargar directamente el ZIP desde GitHub
2. **Ejecutar el proyecto:**
Desde Visual Studio: Abrir MediSchedule_Prototipo.sln y ejecutar( F5 o Run)
Desde consola:
dotnet run --project MediSchedule_Prototipo.csproj
⚠️ Nota: La base de datos medischedule.db es esencial. No eliminar ni mover el archivo
🗂 Estructura del proyecto
Carpeta / Archivo	Descripción
/Data	DbContext y migraciones de SQLite
/Models	Clases de dominio (Usuario, Cita, Rol, etc.)
/Menus	Menús por rol: SuperAdmin, Admin, Personal Médico, Paciente
/Services	Lógica de negocio y acceso a datos
/Utils	Funciones auxiliares y logging
Program.cs	Punto de entrada de la aplicación
medischedule.db	Base de datos SQLite con datos de prueba (fundamental)

⚙️ Funcionamiento general
Al ejecutar el sistema:

Se crea automáticamente el SuperAdmin si no existe (ID: 1, Contraseña: 1234).
El sistema soporta login multirol: cada usuario ve un menú acorde a su rol.
