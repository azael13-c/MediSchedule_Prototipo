# MediSchedule Prototipo

Prototipo en C# de **gestión de citas médicas** con **flujo multirol** en consola.  

---

## 📦 Instalación / Preparación

1. **Clonar el repositorio:**
```bash
git clone https://github.com/azael13-c/MediSchedule_Prototipo.git

cd MediSchedule_Prototipo
O descargar directamente el ZIP desde GitHub.

Ejecutar el proyecto:

Desde Visual Studio: Abrir MediSchedule_Prototipo.sln y ejecutar( F5 o Run).

Desde consola:

bash

dotnet run --project MediSchedule_Prototipo.csproj

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

SuperAdmin es el inicio del sistema: puede crear usuarios, puestos de salud y configurar el entorno.

El sistema soporta login multirol: cada usuario ve un menú acorde a su rol.

🛠 Funcionalidades por rol
👑 SuperAdmin
Crear y listar usuarios y puestos de salud.

Acceder a todos los módulos del sistema.

Control total sobre la base de datos inicial y configuración.

🧑‍💼 Admin
Crear y gestionar usuarios y puestos de salud.

Listar citas y asignarlas a personal médico.

Supervisar el estado de pacientes y médicos.

👩‍⚕️ Personal Médico
Visualizar citas asignadas.

Consultar pacientes y horarios.

Gestionar disponibilidad y agenda.

🧑 Paciente
Visualizar sus citas.

Consultar médicos disponibles.

Navegación simple por menú de consola.

⚠️ Notas importantes
La base de datos medischedule.db debe permanecer en la raíz del proyecto.

Proyecto académico/prototipo, diseñado para demostración de funcionalidades multirol en consola.

Los emojis en menús y mensajes son parte de la interfaz de usuario para mejorar la visualización.
