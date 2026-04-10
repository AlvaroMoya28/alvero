# Sistema de Gestión de Eventos y Citas

Sistema web completo para la gestión de eventos, citas técnicas y reservas, desarrollado con .NET 8 (backend) y Vue.js 3 (frontend).

## 📋 Requisitos Previos

### Backend
- **.NET 8 SDK** o superior
- **Oracle Database** (o acceso a una instancia Oracle)
- **Visual Studio 2022** o **VS Code** con extensión de C#

### Frontend
- **Node.js** (versión 16 o superior)
- **npm** o **yarn**

## 🚀 Instalación y Configuración

### 1. Clonar el Repositorio

```bash
git clone https://git.ucr.ac.cr/ALVARO.MOYAARRIETA/calidadsoftware.git
cd calidadsoftware
```

### 2. Configurar Backend

#### 2.1. Configurar Base de Datos Oracle

Edita el archivo `EventosBackend/appsettings.json` y configura la cadena de conexión:

```json
{
  "ConnectionStrings": {
    "OracleConnection": "User Id=tu_usuario;Password=tu_password;Data Source=tu_host:puerto/servicio;"
  }
}
```

Si usas Oracle Wallet, coloca los archivos del wallet en `EventosBackend/Wallet/`.

En sqlnet.ora cambiar la ruta de del wallet a donde se encuentra el wallet en tu maquina 

#### 2.2. Ejecutar Migraciones

```bash
cd EventosBackend
dotnet ef database update
```

O ejecuta los scripts SQL manualmente desde la carpeta `EventosBackend/Migrations/`.

#### 2.3. Configurar Variables de Entorno (Opcional)

Crea un archivo `appsettings.Development.json` para configuraciones locales:

```json
{
  "Jwt": {
    "SecretKey": "tu-clave-secreta-muy-larga-y-segura-aqui",
    "ValidIssuer": "EventosBackend",
    "ValidAudience": "EventosFrontend",
    "DurationInMinutes": 60
  },
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "Username": "tu-email@gmail.com",
    "Password": "tu-app-password",
    "FromName": "Sistema de Eventos",
    "FromAddress": "tu-email@gmail.com"
  }
}
```

### 3. Configurar Frontend

```bash
cd frontend
npm install
```

Configura la URL del backend en `frontend/src/config.js` o en las variables de entorno:

```javascript
export const API_BASE_URL = 'http://localhost:5000/api';
```

## 🏃 Ejecución

### Opción 1: Ejecutar con Script PowerShell (Recomendado)

Desde la raíz del proyecto:

```powershell
.\start-dev.ps1
```

Este script iniciará automáticamente:
- Backend en `http://localhost:5000`
- Frontend en `http://localhost:8080`

### Opción 2: Ejecutar Manualmente

#### Backend

```bash
cd EventosBackend
dotnet run
```

El backend estará disponible en `http://localhost:5000` o `https://localhost:5001`

#### Frontend

En otra terminal:

```bash
cd frontend
npm run serve
```

El frontend estará disponible en `http://localhost:8080`

## 🧪 Ejecutar Pruebas

### Pruebas Unitarias del Backend

```bash
cd EventosBackend.Tests
dotnet test
```

### Ejecutar pruebas con cobertura de código:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Pruebas del Frontend

```bash
cd frontend
npm run test:unit
```

### Ejecutar todas las pruebas:

```bash
npm test
```

## 📊 Métricas GQM (Goal-Question-Metric)

El proyecto incluye pruebas de métricas GQM para evaluar:

- **Objetivo 2**: Tiempo de carga <= 2000ms
- **Objetivo 3**: Seguridad - Bloqueo de accesos no autorizados >= 95%
- **Objetivo 4**: Concurrencia - Soporte para 100 usuarios simultáneos

Para ejecutar las pruebas GQM:

```bash
cd EventosBackend.Tests
dotnet test --filter "FullyQualifiedName~GQM"
```

### Generar Reporte GQM:

```powershell
.\test-connections.ps1
```

Los reportes se generarán en `TestResults/GQM_<timestamp>/`

## 🔧 Comandos Útiles

### Backend

```bash
# Compilar
dotnet build

# Restaurar paquetes
dotnet restore

# Limpiar compilación
dotnet clean

# Crear migración
dotnet ef migrations add NombreMigracion

# Revertir migración
dotnet ef database update MigracionAnterior
```

### Frontend

```bash
# Compilar para producción
npm run build

# Ejecutar linter
npm run lint

# Formatear código
npm run format
```

## 📁 Estructura del Proyecto

```
calidadsoftware/
├── EventosBackend/              # API Backend (.NET 8)
│   ├── Controllers/             # Controladores API
│   ├── Services/                # Lógica de negocio
│   ├── Repositories/            # Acceso a datos
│   ├── Models/                  # Entidades y DTOs
│   │   ├── Entities/            # Modelos de BD
│   │   ├── DTOs/                # Data Transfer Objects
│   │   └── Context/             # DbContext
│   ├── Migrations/              # Migraciones de BD
│   └── Utilities/               # Utilidades y helpers
├── EventosBackend.Tests/        # Pruebas unitarias
│   ├── Controllers/             # Tests de controladores
│   ├── Services/                # Tests de servicios
│   ├── Security/                # Tests de seguridad
│   └── Performance/             # Tests de rendimiento
├── frontend/                    # Aplicación Vue.js 3
│   ├── src/
│   │   ├── components/          # Componentes Vue
│   │   ├── views/               # Vistas/Páginas
│   │   ├── router/              # Configuración de rutas
│   │   ├── store/               # Vuex store (si aplica)
│   │   └── services/            # Servicios API
│   └── tests/                   # Tests del frontend
└── TestResults/                 # Resultados de pruebas
```

## 🐛 Solución de Problemas

### Error de conexión a Oracle

1. Verifica que Oracle Database esté ejecutándose
2. Confirma las credenciales en `appsettings.json`
3. Si usas Wallet, verifica que los archivos estén en la carpeta correcta

### Error de CORS en el frontend

Verifica que el backend tenga configurado CORS en `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins("http://localhost:8080")
                       .AllowAnyHeader()
                       .AllowAnyMethod());
});
```

### Puerto en uso

Si los puertos 5000 u 8080 están en uso, puedes cambiarlos:

**Backend**: Modifica `EventosBackend/Properties/launchSettings.json`
**Frontend**: Modifica `frontend/vue.config.js`

## 📝 Notas Adicionales

- Las pruebas utilizan bases de datos en memoria (InMemory) para no afectar la BD real
- El sistema incluye autenticación JWT
- Los passwords se almacenan hasheados con salt
- Se recomienda usar HTTPS en producción

## 📞 Soporte

Para reportar problemas o consultas, contacta al equipo de desarrollo.

## 📄 Licencia

Universidad de Costa Rica - Proyecto Académico
