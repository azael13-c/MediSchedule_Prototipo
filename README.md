# MediSchedule Prototipo

Prototipo en C# de **gestión de citas médicas** con **flujo multirol** en consola.

---

## 📋 Requisitos

- .NET SDK 6.0 o superior
- Visual Studio 2022 (o VS Code con extensión de C#)
- SQLite (incluido como `medischedule.db`)

## 📦 Instrucciones de ejecución

1. **Clonar el repositorio:**
   ```bash
   git clone https://github.com/azael13-c/MediSchedule_Prototipo.git

   cd MediSchedule_Prototipo
   
   O puedes descargar el ZIP desde GitHub
   Ejecutar el proyecto:

Desde Visual Studio: Abre MediSchedule_Prototipo.sln y presiona F5 (Run).
Desde consola: 

dotnet run --project MediSchedule_Prototipo.csproj

🗂 Estructura del proyecto

Carpeta / Archivo,Descripción
/Data,DbContext y migraciones de SQLite
/Models,"Clases de dominio (Usuario, Cita, Rol, etc.)"
/Menus,"Menús por rol: SuperAdmin, Admin, Personal Médico, Paciente"
/Services,Lógica de negocio y acceso a datos
/Utils,Funciones auxiliares y logging
Program.cs,Punto de entrada de la aplicación
medischedule.db,Base de datos SQLite con datos de prueba
⚙️ Funcionamiento general

Se crea un SuperAdmin automáticamente (ID: 1, Contraseña: 1234).
Cada usuario accede a un menú según su rol (SuperAdmin, Admin, Personal Médico, Paciente).

⚠️ Notas importantes

La base de datos medischedule.db debe estar en la raíz del proyecto.
Usa las credenciales de SuperAdmin para iniciar.
Si la base de datos no carga, verifica que medischedule.db esté en la raíz.
