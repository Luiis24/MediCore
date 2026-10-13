# 🩺 MediCore

**Sistema web de gestión clínica básica**: pacientes, citas médicas e historias clínicas, con un dashboard de métricas. Construido con **ASP.NET Core 8 (MVC)**, **Entity Framework Core** y **SQLite**, y desplegado con **Docker en Render**.

<img width="1366" height="633" alt="image" src="https://github.com/user-attachments/assets/03f4c043-513d-4ac6-be3b-a36d19fb1189" />

🌐 **Demo en vivo:** [https://medicore-304b.onrender.com/](https://medicore-304b.onrender.com/)

> ⏱️ Si el servicio está en un plan gratuito de Render, la primera carga puede tardar unos segundos mientras el contenedor "despierta".

---

## 📑 Tabla de contenidos

1. [¿Qué es MediCore?](#-qué-es-medicore)
2. [Funcionalidades](#-funcionalidades)
3. [Lo que MediCore NO hace](#-lo-que-medicore-no-hace-alcance-actual)
4. [Stack tecnológico](#-stack-tecnológico)
5. [Qué NO usa](#-qué-no-usa)
6. [Arquitectura y estructura del proyecto](#-arquitectura-y-estructura-del-proyecto)
7. [Modelo de datos](#-modelo-de-datos)
8. [Rutas web (MVC)](#-rutas-web-mvc)
9. [API REST](#-api-rest)
10. [Interfaz de usuario](#-interfaz-de-usuario)
11. [Base de datos, migraciones y datos semilla](#-base-de-datos-migraciones-y-datos-semilla)
12. [Ejecución local](#-ejecución-local)
13. [Ejecución con Docker](#-ejecución-con-docker)
14. [Despliegue en Render](#-despliegue-en-render)
15. [Configuración](#-configuración)
16. [Consideraciones y deuda técnica conocida](#-consideraciones-y-deuda-técnica-conocida)
17. [Hoja de ruta sugerida](#-hoja-de-ruta-sugerida)
18. [Historial del repositorio](#-historial-del-repositorio)

---

## 🏥 ¿Qué es MediCore?

MediCore es una aplicación web monolítica de **renderizado en servidor** (Server-Side Rendering con Razor) pensada como un sistema médico de apoyo: permite registrar pacientes, programar y gestionar citas, y consultar el expediente clínico de cada paciente, todo desde un panel con métricas rápidas.

Además de las vistas HTML, expone una pequeña **API REST en JSON** (pacientes, citas, estadísticas e historias clínicas) pensada para ser consumida por otros clientes o integraciones.

Es un proyecto de **alcance acotado y didáctico/demostrativo**: cubre el flujo central (pacientes → citas → historia clínica) sin pretender ser un HIS/EMR completo. Los datos de ejemplo son ficticios.

---

## ✨ Funcionalidades

### Dashboard (`/`)
- 4 tarjetas de métricas calculadas en tiempo real desde la base de datos:
  - **Total de pacientes**
  - **Pacientes activos** (`Status == "Activo"`)
  - **Citas totales**
  - **Citas de hoy** (`AppointmentDate.Date == DateTime.Today`)
- Tabla con las **5 últimas citas** (ordenadas por fecha descendente), con paciente, fecha/hora, motivo y estado (badge de color).
- **Acciones rápidas sobre citas "Programada"**: marcar como **Completada** ✅ o **Cancelada** ❌ con un clic. Las citas que ya no están programadas muestran "Sin acciones".
- Botón de acceso directo para **programar una nueva cita**.

### Pacientes (`/Patients`)
- **Directorio** con todos los pacientes: nombre, documento, edad, género, diagnóstico principal y estado (Activo/Inactivo).
- **Buscador** por **nombre o número de documento** (búsqueda por coincidencia parcial, vía `GET ?searchString=`).
- **Registro de nuevo paciente** con validación de servidor (nombre, documento, edad y diagnóstico obligatorios; género y estado seleccionables).
- **Detalle / expediente médico** de cada paciente:
  - Datos demográficos (documento, edad, género, diagnóstico base, estado, fecha de registro).
  - **Evoluciones clínicas** en un acordeón, ordenadas de la más reciente a la más antigua, mostrando fecha, médico tratante y notas de evolución.

### Citas
- **Programar cita** (`/Home/CreateAppointment`): seleccionar paciente, fecha y hora (`datetime-local`), motivo de consulta y estado inicial (*Programada* o *Completada*).
- Ciclo de estados: `Programada` → `Completada` | `Cancelada`.

### API JSON
- Listado y creación de pacientes, listado de citas (con paciente incluido), endpoint de estadísticas del dashboard y consulta de historia clínica por paciente. Ver [API REST](#-api-rest).

### Experiencia de uso
- **Modo oscuro / claro** con preferencia persistida en `localStorage`.
- **Diseño responsive**: barra lateral fija en escritorio y menú *offcanvas* (hamburguesa) en móvil.
- Interfaz completamente en **español**.

### Despliegue y arranque
- **Migraciones automáticas** al iniciar la aplicación (crea la base de datos y tablas si no existen).
- **Código de datos semilla** (3 pacientes de ejemplo) incluido en `Program.cs`, aunque por un problema de orden en el archivo hoy **no llega a ejecutarse** (ver [Datos semilla](#datos-semilla)).
- **Dockerfile multi-stage** listo para producción.

---

## 🚫 Lo que MediCore NO hace (alcance actual)

Para evitar malentendidos, esto es lo que **no** está implementado:

| Área | Estado |
|---|---|
| **Autenticación / login / registro de usuarios** | ❌ No existe. Cualquiera con la URL accede a todo. |
| **Roles y permisos** (médico, recepción, admin) | ❌ No existen. |
| **Editar o eliminar pacientes** | ❌ Solo se pueden crear y consultar. |
| **Editar o eliminar citas** | ❌ Solo crear y cambiar estado (Programada → Completada/Cancelada). |
| **Crear evoluciones clínicas (historia clínica) desde la UI o la API** | ❌ Solo se **leen**; no hay formulario ni endpoint de creación. |
| **Calendario / agenda visual** | ❌ No hay vista de calendario. |
| **Listado completo de citas en la UI** | ❌ El dashboard muestra solo las 5 más recientes. |
| **Gráficos / reportes** | ❌ Solo tarjetas numéricas (aunque los modelos mencionan "gráficos" como idea futura). |
| **Prescripciones, laboratorio, facturación, inventario** | ❌ Fuera de alcance. |
| **Notificaciones (email/SMS/WhatsApp)** | ❌ No existen. |
| **Paginación** | ❌ Los listados se cargan completos. |
| **Detección de documentos duplicados** | ❌ No hay restricción de unicidad en `DocumentNumber`. |
| **Validación de conflictos de agenda** (misma hora, doble cita) | ❌ No hay. |
| **Soporte multi-clínica / multi-tenant** | ❌ No. |
| **Internacionalización (i18n)** | ❌ UI solo en español. |
| **Documentación de API (Swagger/OpenAPI)** | ❌ No incluida. |
| **Pruebas automatizadas** (unitarias/integración) | ❌ No hay proyecto de tests. |
| **Cumplimiento normativo** (HIPAA, Habeas Data / Ley 1581, historia clínica Res. 1995/1999, etc.) | ❌ **No** está diseñado ni certificado para manejar datos clínicos reales. |

> ⚠️ **Importante:** MediCore es un proyecto de demostración. **No lo uses con datos reales de pacientes** sin añadir antes autenticación, autorización, cifrado, auditoría y cumplimiento legal.

---

## 🧰 Stack tecnológico

### Backend
| Tecnología | Versión | Uso |
|---|---|---|
| **.NET / ASP.NET Core** | 8.0 (`net8.0`) | Framework web (MVC + controladores API) |
| **C#** | (por defecto de .NET 8) | Lenguaje, con `Nullable` e `ImplicitUsings` habilitados |
| **Entity Framework Core** | 8.0.11 | ORM y migraciones (*Code First*) |
| `Microsoft.EntityFrameworkCore.Sqlite` | 8.0.11 | Proveedor de base de datos SQLite |
| `Microsoft.EntityFrameworkCore.Design` | 8.0.11 | Herramientas de diseño/migraciones (`PrivateAssets=all`) |
| **System.Text.Json** | incluido | Serialización JSON, con `ReferenceHandler.IgnoreCycles` |

### Base de datos
- **SQLite** (archivo `medicore.db`), cadena de conexión `Data Source=medicore.db`.

### Frontend
| Tecnología | Versión | Origen | Uso |
|---|---|---|---|
| **Razor (.cshtml)** | — | Servidor | Motor de vistas y *Tag Helpers* |
| **Bootstrap** | 5.3.2 | CDN (jsDelivr) | Estilos, grid, componentes (cards, accordion, offcanvas, badges) |
| **Bootstrap Icons** | 1.11.1 | CDN (jsDelivr) | Iconografía |
| **Bootstrap JS Bundle** | 5.3.2 | CDN (jsDelivr) | Acordeón, offcanvas |
| **jQuery** | 3.7.1 | CDN (code.jquery.com) | Cargado en el layout |
| JavaScript vanilla | — | Inline en `_Layout.cshtml` | Alternancia de modo oscuro/claro |

> `wwwroot/lib/` también incluye copias locales de Bootstrap, jQuery, jQuery Validation y jQuery Validation Unobtrusive (plantilla estándar de ASP.NET Core MVC).

### DevOps
- **Docker** (multi-stage: `mcr.microsoft.com/dotnet/sdk:8.0` → `mcr.microsoft.com/dotnet/aspnet:8.0`).
- **Render** (despliegue como servicio web con Docker).
- **Git / GitHub** para control de versiones.

---

## ❎ Qué NO usa

- ❌ **No** usa SQL Server, PostgreSQL ni MySQL (solo SQLite).
- ❌ **No** usa ASP.NET Identity ni ningún sistema de autenticación.
- ❌ **No** usa frameworks SPA (React, Angular, Vue, Blazor): todo es MVC con Razor.
- ❌ **No** usa Swagger/Swashbuckle ni OpenAPI.
- ❌ **No** usa AutoMapper, MediatR, FluentValidation, ni patrón Repository/Unit of Work: los controladores acceden directamente a `AppDbContext`.
- ❌ **No** usa DTOs / ViewModels propios (salvo `ErrorViewModel` de la plantilla): se usan las entidades directamente.
- ❌ **No** usa bibliotecas de gráficos (Chart.js, etc.).
- ❌ **No** usa caché, colas, workers en segundo plano ni SignalR.
- ❌ **No** usa Node.js, npm, webpack ni bundlers.
- ❌ **No** usa Docker Compose ni orquestadores.
- ❌ **No** tiene pipeline de CI/CD propio (el despliegue lo gestiona Render a partir del repositorio).

---

## 🗂️ Arquitectura y estructura del proyecto

Patrón **MVC** clásico de ASP.NET Core, con acceso a datos directo mediante EF Core.

```
MediCore/
├── Controllers/
│   ├── HomeController.cs            # Dashboard, crear cita, actualizar estado de cita (MVC)
│   ├── PatientsController.cs        # Listado/búsqueda, detalle y alta de pacientes (MVC)
│   ├── PatientsApiController.cs     # API REST: GET/POST api/patients
│   ├── AppointmentsController.cs    # API REST: GET api/appointments y /stats
│   └── MedicalRecordsController.cs  # API REST: GET api/medical-records/patient/{id}
├── Models/
│   ├── Pacient.cs                   # Clase Patient
│   ├── Appointment.cs               # ⚠ Contiene la clase MedicalRecord (ver nota)
│   ├── MedicalRecord.cs             # ⚠ Contiene la clase Appointment (ver nota)
│   └── ErrorViewModel.cs
├── data/
│   └── AppDbContext.cs              # DbContext con DbSet<Patient/Appointment/MedicalRecord>
├── Migrations/
│   ├── 20261013024028_InitialCreate.cs                 # Tabla Patients
│   ├── 20261013030512_AddAppointmentsAndRecords.cs     # Status + Appointments + MedicalRecords
│   └── AppDbContextModelSnapshot.cs
├── Views/
│   ├── Home/        # Index (dashboard), CreateAppointment, Privacy
│   ├── Patients/    # Index, Create, Details
│   ├── Shared/      # _Layout, Error, _ValidationScriptsPartial
│   ├── _ViewImports.cshtml
│   └── _ViewStart.cshtml
├── wwwroot/         # css/site.css, js/site.js, favicon.ico, lib/ (Bootstrap, jQuery...)
├── Properties/launchSettings.json   # Perfiles http/https para desarrollo
├── Program.cs                       # Configuración, migraciones automáticas y (intento de) semilla
├── appsettings.json / appsettings.Development.json
├── Dockerfile
├── MediCore.csproj
└── medicore.db                      # Base de datos SQLite incluida en el repositorio
```

> **Nota sobre `Models/`:** los archivos `Appointment.cs` y `MedicalRecord.cs` tienen su contenido **intercambiado** (el primero declara `MedicalRecord` y el segundo `Appointment`). Funciona porque C# no exige que el nombre del archivo coincida con el de la clase, pero conviene renombrarlos.

---

## 🧬 Modelo de datos

Tres entidades con relaciones 1‑N desde `Patient` y borrado en cascada.

```
Patient (1) ──< Appointment (N)
Patient (1) ──< MedicalRecord (N)
```

### `Patient` → tabla `Patients`
| Campo | Tipo | Notas |
|---|---|---|
| `Id` | int (PK, autoincremental) | |
| `Name` | string | **Requerido** — "El nombre es obligatorio" |
| `DocumentNumber` | string | **Requerido** — "El documento es obligatorio" |
| `Age` | int | **Requerido** — "La edad es obligatoria" |
| `Gender` | string | Masculino / Femenino / Otro (desde el formulario) |
| `Diagnosis` | string | **Requerido** — "El diagnóstico es obligatorio" |
| `Status` | string | `Activo` (por defecto) o `Inactivo` |
| `RegistrationDate` | DateTime | Por defecto `DateTime.Now` |
| `Appointments` | ICollection | Navegación |
| `MedicalRecords` | ICollection | Navegación |

### `Appointment` → tabla `Appointments`
| Campo | Tipo | Notas |
|---|---|---|
| `Id` | int (PK) | |
| `PatientId` | int (FK → Patients, cascade) | |
| `AppointmentDate` | DateTime | **Requerido** |
| `Status` | string | `Programada` (por defecto), `Completada`, `Cancelada` |
| `Reason` | string | Motivo de la consulta |

### `MedicalRecord` → tabla `MedicalRecords`
| Campo | Tipo | Notas |
|---|---|---|
| `Id` | int (PK) | |
| `PatientId` | int (FK → Patients, cascade) | |
| `RecordDate` | DateTime | Por defecto `DateTime.Now` |
| `Notes` | string | **Requerido** — notas de evolución |
| `DoctorName` | string | Por defecto `"Dr. House"` |

Índices: `IX_Appointments_PatientId` e `IX_MedicalRecords_PatientId`.

---

## 🧭 Rutas web (MVC)

Ruta por defecto: `{controller=Home}/{action=Index}/{id?}`

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/` | Dashboard con métricas y últimas 5 citas |
| GET | `/Home/CreateAppointment` | Formulario para programar cita |
| POST | `/Home/CreateAppointment` | Guarda la cita (con `[ValidateAntiForgeryToken]`) |
| POST | `/Home/UpdateStatus` | Cambia el estado de una cita (`id`, `status`) |
| GET | `/Home/Privacy` | Vista de privacidad (plantilla por defecto, sin contenido real) |
| GET | `/Patients` | Directorio de pacientes (`?searchString=`) |
| GET | `/Patients/Details/{id}` | Expediente médico del paciente |
| GET | `/Patients/Create` | Formulario de alta de paciente |
| POST | `/Patients/Create` | Guarda el paciente (con `[ValidateAntiForgeryToken]`) |

---

## 🔌 API REST

Controladores con `[ApiController]` y respuestas en JSON. Los ciclos de referencia se ignoran (`ReferenceHandler.IgnoreCycles`).

| Método | Endpoint | Descripción | Respuestas |
|---|---|---|---|
| GET | `/api/patients` | Lista todos los pacientes | `200` |
| POST | `/api/patients` | Crea un paciente (JSON del modelo `Patient`) | `201` / `400` |
| GET | `/api/appointments` | Lista todas las citas **incluyendo** el paciente | `200` |
| GET | `/api/appointments/stats` | Estadísticas del dashboard | `200` |
| GET | `/api/medical-records/patient/{patientId}` | Historia clínica de un paciente, de la más reciente a la más antigua | `200` / `404` |

### Ejemplos

```bash
# Listar pacientes
curl https://medicore-304b.onrender.com/api/patients

# Estadísticas
curl https://medicore-304b.onrender.com/api/appointments/stats
# → { "totalPatients": N, "activePatients": N, "totalAppointments": N, "todayAppointments": N }

# Crear paciente
curl -X POST https://medicore-304b.onrender.com/api/patients \
  -H "Content-Type: application/json" \
  -d '{
        "name": "Laura Díaz",
        "documentNumber": "10203040",
        "age": 36,
        "gender": "Femenino",
        "diagnosis": "Migraña crónica",
        "status": "Activo"
      }'

# Historia clínica de un paciente
curl https://medicore-304b.onrender.com/api/medical-records/patient/1
# 404 con { "message": "No se encontraron registros clínicos para este paciente." } si no hay registros
```

> Los valores de `/stats` dependen de los datos que existan en el momento de la consulta.

---

## 🎨 Interfaz de usuario

- **Layout** con barra lateral (`Dashboard`, `Pacientes`) y botón de modo oscuro en la parte inferior.
- **Móvil:** barra superior con botón hamburguesa que abre un menú lateral *offcanvas*.
- **Tema:** `data-bs-theme` de Bootstrap 5.3 (`light`/`dark`), guardado en `localStorage` bajo la clave `theme`.
- **Badges de estado:**
  - Citas: 🟢 Completada · 🔵 Programada · 🔴 Cancelada
  - Pacientes: 🟢 Activo · ⚪ Inactivo
- **Iconos** de Bootstrap Icons en botones, tarjetas y navegación.

---

## 💾 Base de datos, migraciones y datos semilla

### Migraciones
Dos migraciones *Code First*:
1. `InitialCreate` — crea `Patients`.
2. `AddAppointmentsAndRecords` — agrega `Patients.Status` y crea `Appointments` y `MedicalRecords` con sus índices y claves foráneas.

Al arrancar, `Program.cs` ejecuta `Database.Migrate()`, por lo que **no necesitas correr `dotnet ef database update` manualmente**: la base y las tablas se crean/actualizan solas.

### Datos semilla
`Program.cs` contiene un bloque que, si la tabla `Patients` está vacía, inserta 3 pacientes ficticios:

| Nombre | Documento | Edad | Género | Diagnóstico |
|---|---|---|---|---|
| Carlos Pérez | 11223344 | 32 | Masculino | Hipertensión arterial controlada |
| María Gómez | 55667788 | 28 | Femenino | Control prenatal regular |
| Juan Rodríguez | 99001122 | 45 | Masculino | Diabetes Mellitus Tipo 2 |

La semilla **solo crearía pacientes**; no genera citas ni evoluciones clínicas.

> ⚠️ **Estado real:** ese bloque está escrito *después* del primer `app.Run()` de `Program.cs`, que bloquea hasta que la aplicación se detiene. Por eso, **en la práctica la semilla no se ejecuta**: una base nueva queda con las tablas vacías y los pacientes hay que crearlos desde la UI o la API. Moviendo el bloque antes de `app.Run()` (y eliminando el segundo `Run()` duplicado) se activaría.

### El archivo `medicore.db` del repositorio
El repositorio incluye un `medicore.db` con datos de desarrollo (8 pacientes, 5 citas y 4 evoluciones clínicas con médicos de ejemplo). Es una copia local de trabajo; no está configurado como archivo de publicación, por lo que el entorno desplegado crea su propia base al iniciar (y, por tanto, no necesariamente contiene estos datos).

### Comandos útiles de EF Core

```bash
dotnet tool install --global dotnet-ef

# Crear una nueva migración
dotnet ef migrations add NombreDeLaMigracion

# Aplicar migraciones manualmente (opcional; la app ya las aplica al iniciar)
dotnet ef database update
```

---

## 💻 Ejecución local

### Requisitos
- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) o superior
- Git

### Pasos

```bash
git clone https://github.com/Luiis24/MediCore.git
cd MediCore
dotnet restore
dotnet run
```

Perfiles de `launchSettings.json`:
- **http:** `http://localhost:5290`
- **https:** `https://localhost:7262` y `http://localhost:5290`

Para usar el perfil HTTPS:

```bash
dotnet run --launch-profile https
```

Si es la primera vez con HTTPS en desarrollo: `dotnet dev-certs https --trust`.

La base SQLite se crea (o se actualiza) automáticamente en el directorio de trabajo. Si usas el `medicore.db` incluido en el repo, ya trae datos de ejemplo; con una base nueva empezarás sin pacientes (ver [Datos semilla](#datos-semilla)).

---

## 🐳 Ejecución con Docker

El `Dockerfile` es *multi-stage*:

1. **build:** imagen `dotnet/sdk:8.0` → `dotnet restore` + `dotnet publish -c Release`.
2. **final:** imagen `dotnet/aspnet:8.0` con la salida publicada.
3. Expone el puerto **8080** y define `ASPNETCORE_URLS=http://+:8080`.

```bash
docker build -t medicore .
docker run -p 8080:8080 medicore
# → http://localhost:8080
```

Para conservar los datos entre reinicios del contenedor, monta un volumen o usa una ruta de base de datos persistente (ver [Configuración](#-configuración)).

---

## ☁️ Despliegue en Render

La aplicación está desplegada en **https://medicore-304b.onrender.com/** como servicio web basado en el `Dockerfile` del repositorio.

Pasos típicos para replicarlo:
1. Crear un **Web Service** en [Render](https://render.com) conectado a este repositorio.
2. Seleccionar **Docker** como entorno (Render detecta el `Dockerfile`).
3. Render debe enrutar al puerto **8080** (el configurado en el contenedor).
4. Desplegar. Al iniciar, la app aplica migraciones y crea las tablas vacías (la semilla no se ejecuta, ver [Datos semilla](#datos-semilla)).

> 💡 **Persistencia en Render:** el sistema de archivos de un servicio web de Render es **efímero** salvo que se agregue un *Persistent Disk*. Con SQLite en el contenedor, los datos creados en producción pueden **reiniciarse** tras un nuevo despliegue o reinicio del contenedor. Para persistencia real, adjunta un disco y apunta la cadena de conexión a esa ruta, o migra a PostgreSQL.

---

## ⚙️ Configuración

**`appsettings.json`**
```json
{
  "Logging": { "LogLevel": { "Default": "Information", "Microsoft.AspNetCore": "Warning" } },
  "AllowedHosts": "*",
  "ConnectionStrings": { "DefaultConnection": "Data Source=medicore.db" }
}
```

Puedes sobrescribir la cadena de conexión con una variable de entorno (convención estándar de ASP.NET Core):

```bash
ConnectionStrings__DefaultConnection="Data Source=/data/medicore.db"
```

| Variable | Descripción |
|---|---|
| `ASPNETCORE_ENVIRONMENT` | `Development` activa la página de errores detallada; en otro caso se usa `/Home/Error` y HSTS |
| `ASPNETCORE_URLS` | URLs/puerto de escucha (en Docker: `http://+:8080`) |
| `ConnectionStrings__DefaultConnection` | Ruta de la base SQLite |

---

## 🔍 Consideraciones y deuda técnica conocida

Revisión honesta del código actual, útil para quien quiera contribuir:

**Seguridad**
- Sin autenticación ni autorización: todas las páginas y endpoints son públicos, incluida la **creación de pacientes vía API** (`POST /api/patients`).
- `POST /Home/UpdateStatus` no valida token antiforgery explícitamente ni restringe valores: acepta cualquier cadena en `status`.
- Los endpoints de la API reciben la entidad `Patient` directamente (riesgo de *over-posting*); conviene usar DTOs.
- Los datos médicos se muestran sin ningún control de acceso ni auditoría.

**Código**
- **`Program.cs` tiene bloques duplicados:** `Database.Migrate()` se escribe dos veces (solo corre la primera), hay dos llamadas a `app.Run()` (el bloque de semilla queda después del primer `Run()`, por lo que **nunca se ejecuta** y la app no siembra datos) y dos `MapControllerRoute`. Además, `UseStaticFiles()` aparece después de `UseRouting()`/`UseAuthorization()` y repetido. Conviene unificar: migrar → sembrar → pipeline → un solo `Run()`.
- Los archivos `Appointment.cs` y `MedicalRecord.cs` están intercambiados respecto a las clases que contienen.
- El archivo del modelo se llama `Pacient.cs` (typo) y contiene `Patient`.
- La carpeta `data/` está en minúscula mientras el namespace es `MediCore.Data`.
- `Appointments.AppointmentDate` se compara con `DateTime.Today`; el resultado depende de la zona horaria del servidor (UTC en contenedores), lo que puede desfasar "Citas de hoy".
- La migración `InitialCreate` no incluye `Status`, y la segunda lo agrega con valor por defecto `""`; los pacientes antiguos pueden quedar con estado vacío.
- Las migraciones y el *snapshot* están generados con una versión de EF Core distinta (`10.0.12`) a la declarada en el `.csproj` (`8.0.11`). Conviene alinear versiones de paquetes y herramienta `dotnet-ef`.
- La validación en cliente (jQuery Validation) no está activa en los formularios porque no se incluye `_ValidationScriptsPartial` en las vistas; la validación funciona solo en servidor.
- La vista `Privacy` y la página `Error` son las plantillas por defecto sin personalizar; `site.js` está vacío.
- `RegistrationDate`, `RecordDate` y `DoctorName` tienen valores por defecto fijos (`DateTime.Now`, `"Dr. House"`).
- Los controladores API están acoplados a `AppDbContext` (sin capa de servicios).

**Repositorio**
- Se versionan directorios de **`bin/` y `obj/`** y el archivo **`medicore.db`**, y no existe `.gitignore`. Se recomienda agregarlo (por ejemplo con `dotnet new gitignore`) y quitar esos archivos del control de versiones (`git rm -r --cached bin obj medicore.db`).
- No hay licencia declarada.

---

## 🛣️ Hoja de ruta sugerida

- [ ] Autenticación y roles con ASP.NET Core Identity
- [ ] CRUD completo de pacientes y citas (editar/eliminar)
- [ ] Formulario y endpoint para crear evoluciones clínicas
- [ ] Vista de agenda/calendario y listado completo de citas con filtros y paginación
- [ ] Gráficos en el dashboard (citas por estado/día, pacientes por edad)
- [ ] Unicidad de documento y validación de conflictos de agenda
- [ ] DTOs, capa de servicios y documentación con Swagger
- [ ] Pruebas unitarias e de integración (xUnit)
- [ ] Persistencia robusta (PostgreSQL o disco persistente en Render)
- [ ] CI/CD con GitHub Actions
- [ ] Limpieza de `Program.cs`, nombres de archivos y `.gitignore`

---

## 🕓 Historial del repositorio

| Commit | Descripción |
|---|---|
| `594f1fe` | Creación de MediCore completo |
| `ca4fe8e` | Creación del archivo Dockerfile |
| `7bb4be0` | Cambio de versión para desplegar en Render |
| `683ca36` | Más cambios de versión para desplegar en Render |
| `5015104` | Migración al iniciar para producción |

---

## 👤 Autor

**Luiis24** — [github.com/Luiis24](https://github.com/Luiis24)

---

<p align="center">Hecho con ❤️ y .NET 8 · <a href="https://medicore-304b.onrender.com/">Ver demo</a></p>
