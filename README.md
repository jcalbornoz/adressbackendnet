# Prueba Técnica ADRES – Backend .NET + SQL Server

Esta versión implementa el backend en **ASP.NET Core (.NET 8)** con **Entity Framework Core** y **SQL Server**, manteniendo:

- Endpoints REST:
  - `/api/catalogs`, `/api/catalogs/xml`
  - `/api/acquisitions`, `/api/acquisitions/{id}`, `/api/acquisitions/{id}/status`, `/api/acquisitions/{id}/history`
- Frontend estático en `wwwroot` (mismo HTML/JS/CSS que hemos venido usando).
- Cálculo de **Valor Total** como `cantidad * valorUnitario` en frontend y backend.
- Persistencia completa en **SQL Server**.

## Estructura del proyecto

```text
adres-prueba-tecnica-dotnet/
  backend/
    Adres.Prueba.Api.csproj
    Program.cs
    appsettings.json
    Properties/
      launchSettings.json
    Data/
      AppDbContext.cs
    Models/
      Acquisition.cs
      AcquisitionHistory.cs
      UnidadAdministrativa.cs
      TipoBienServicio.cs
    Controllers/
      AcquisitionsController.cs
      CatalogsController.cs
    wwwroot/
      index.html
      styles.css
      app.js
```

## Requisitos

- **.NET 8 SDK**
- **SQL Server** local (Developer, Express o LocalDB)

La cadena de conexión por defecto en `appsettings.json` es:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=AdresPruebaAdquisiciones;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

Ajústala según tu entorno (por ejemplo: `Server=.\\SQLEXPRESS;...`).

## Pasos para levantar el backend

1. Abrir una terminal en la carpeta `backend`:

   ```bash
   cd backend
   ```

2. Restaurar paquetes:

   ```bash
   dotnet restore
   ```

3. Crear las migraciones e inicializar la base de datos (requiere tener instalada la herramienta de EF):

   ```bash
   dotnet tool install --global dotnet-ef      # solo si no la tienes
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

   Esto creará la base de datos `AdresPruebaAdquisiciones` en tu SQL Server con:

   - Tabla `Acquisitions`
   - Tabla `Histories`
   - Tabla `UnidadesAdministrativas` (con datos de ejemplo)
   - Tabla `TiposBienServicio` (con datos de ejemplo)

4. Ejecutar la API:

   ```bash
   dotnet run
   ```

5. Navegar a:

   - Frontend: `https://localhost:7143` (o el puerto HTTP que indique la consola, por ejemplo `http://localhost:5143`)
   - Swagger: `https://localhost:7143/swagger`

El frontend está servido desde `wwwroot` y consume los endpoints `/api/...` del backend.

## Endpoints principales

### Catálogos

- `GET /api/catalogs`  
  Devuelve:

  ```json
  {
    "unidadesAdministrativas": ["Dirección General", "..."],
    "tiposBienServicio": ["Medicamentos", "..."]
  }
  ```

- `GET /api/catalogs/xml`  
  Devuelve el mismo contenido en formato XML.

### Adquisiciones

- `GET /api/acquisitions`  
  Soporta filtros opcionales vía query string:
  - `unidad`, `tipo`, `proveedor`
  - `estado` (`ACTIVO` / `INACTIVO`)
  - `fechaDesde`, `fechaHasta` (ISO, ej. `2025-11-29`)

- `POST /api/acquisitions`  
  Body:

  ```json
  {
    "presupuesto": 10000000,
    "unidad": "Oficina de Tecnologías de la Información",
    "tipo": "Servicios de tecnología",
    "cantidad": 5,
    "valorUnitario": 2000000,
    "fechaAdquisicion": "2025-11-30",
    "proveedor": "Proveedor XYZ",
    "documentacion": "OC-123, Contrato 2025-001"
  }
  ```

  > El backend calcula siempre `valorTotal = cantidad * valorUnitario`.

- `PUT /api/acquisitions/{id}`  
  Mismo body que `POST`, recalculando `valorTotal`.

- `PATCH /api/acquisitions/{id}/status`  

  ```json
  { "activo": true }
  ```

- `GET /api/acquisitions/{id}/history`  
  Lista de eventos de historial asociados al registro.

## Lógica de negocio

- Validaciones:
  - `presupuesto >= 0`
  - `cantidad > 0`
  - `valorUnitario >= 0`
  - `unidad`, `tipo`, `proveedor` obligatorios.
  - `fechaAdquisicion` válida.
- `valorTotal` **no se ingresa manualmente**:
  - Se calcula en el frontend para mostrarle al usuario.
  - Se calcula nuevamente en el backend para garantizar integridad.
- Se registra historial en:
  - Creación (`CREADO`)
  - Actualización (`ACTUALIZADO`)
  - Cambio de estado (`ESTADO`)

