# AutoTallerManager — Backend API

> Sistema integral de gestión para talleres automotrices. API RESTful construida con ASP.NET Core 9, arquitectura hexagonal, CQRS y PostgreSQL.

**Repositorio Frontend:** [FRONTED\_PROYECTO](https://github.com/Ksanti-monsalve/FRONTED_PROYECTO)

---

## Tabla de contenidos

1. [Descripción general](#descripción-general)
2. [Stack tecnológico](#stack-tecnológico)
3. [Arquitectura](#arquitectura)
4. [Estructura del proyecto](#estructura-del-proyecto)
5. [Módulos y entidades](#módulos-y-entidades)
6. [Flujos de negocio](#flujos-de-negocio)
7. [Autenticación y roles](#autenticación-y-roles)
8. [Endpoints principales](#endpoints-principales)
9. [Configuración](#configuración)
10. [Instalación y ejecución](#instalación-y-ejecución)
11. [Usuarios de prueba](#usuarios-de-prueba)

---

## Descripción general

AutoTallerManager es un sistema empresarial completo para la gestión de talleres automotrices. Cubre el ciclo completo del servicio: desde la creación de presupuestos hasta la facturación consolidada, pasando por gestión de inventario, asignación de mecánicos, agenda de citas y seguimiento de órdenes de trabajo.

---

## Stack tecnológico

| Categoría | Tecnología | Versión |
|-----------|-----------|---------|
| Framework | ASP.NET Core | 9.0 |
| ORM | Entity Framework Core + Npgsql | 9.0 |
| Base de datos | PostgreSQL | 16+ |
| CQRS / Mediator | MediatR | 12.4 |
| Mapeo de objetos | AutoMapper | 13.0 |
| Validación | FluentValidation | 11.11 |
| Autenticación | JWT Bearer | 9.0 |
| Hash contraseñas | BCrypt.Net-Next | 4.0 |
| Logging | Serilog (console + file) | 4.2 |
| Rate Limiting | AspNetCoreRateLimit | 5.0 |
| Documentación API | Swashbuckle / Swagger | 7.3 |
| CLI interactiva | Spectre.Console | 0.49 |

---

## Arquitectura

El proyecto sigue **Arquitectura Hexagonal (Ports & Adapters)** combinada con **Clean Architecture** y principios de **DDD**:

```
┌──────────────────────────────────────────────────────────┐
│                        API Layer                          │
│      Controllers · Middleware · Swagger · JWT Config      │
├──────────────────────────────────────────────────────────┤
│                   Application Layer                       │
│   CQRS Commands/Queries · DTOs · Validators · Mappings   │
├──────────────────────────────────────────────────────────┤
│                     Domain Layer                          │
│     Entities · Enums · Interfaces · Domain Exceptions     │
├──────────────────────────────────────────────────────────┤
│                 Infrastructure Layer                      │
│   EF Core DbContext · Migrations · Seeds · JWT Service   │
└──────────────────────────────────────────────────────────┘
```

**Patrones aplicados:**
- **CQRS** — separación de lecturas (Queries) y escrituras (Commands) con MediatR
- **Repository Pattern** — acceso a datos a través de `IApplicationDbContext`
- **Soft Delete** — entidades marcadas con `Eliminado = true` en lugar de eliminarse
- **Pipeline Behaviors** — validación y logging automático en cada comando/query

---

## Estructura del proyecto

```
AutoTallerManager/
│
├── Api/                            ← Capa de presentación
│   ├── Controllers/                ← 15+ controladores REST
│   ├── Extensions/                 ← Configuración JWT, Swagger
│   ├── Helpers/
│   │   ├── Errors/                 ← Middleware global de excepciones
│   │   ├── Politicas.cs            ← Políticas de autorización
│   │   └── Roles.cs                ← Constantes de roles
│   └── Program.cs                  ← Entry point y configuración de servicios
│
├── Application/                    ← Lógica de negocio (CQRS)
│   ├── Abstractions/               ← Interfaces: IApplicationDbContext, IJwtService
│   ├── Common/
│   │   ├── Behaviors/              ← ValidationBehavior, LoggingBehavior
│   │   ├── Exceptions/             ← NotFoundException, ForbiddenAccessException
│   │   ├── ApiResponse.cs          ← Envelope de respuesta estándar
│   │   └── PagedResult.cs          ← Resultado paginado genérico
│   ├── Mappings/MappingProfile.cs  ← Perfiles AutoMapper
│   └── UseCase/                    ← Casos de uso por módulo
│       ├── Auth/                   ← Login, Register, RefreshToken
│       ├── Clientes/               ← CRUD clientes
│       ├── Vehiculos/              ← CRUD vehículos con filtro por propietario
│       ├── Ordenes/                ← Ciclo de vida de órdenes de servicio
│       ├── MiniOrdenes/            ← Flujo M-J-C (presupuestos)
│       ├── Repuestos/              ← Catálogo de repuestos
│       ├── Inventario/             ← Entradas, salidas, ajustes de stock
│       ├── Facturas/               ← Facturación consolidada y pagos
│       ├── Citas/                  ← Agendamiento de citas
│       ├── Dashboard/              ← Métricas y reportes
│       └── Catalogos/              ← Catálogos del sistema
│
├── Domain/                         ← Núcleo del dominio (sin dependencias externas)
│   ├── Entities/                   ← 70+ entidades de dominio
│   ├── Enums/                      ← EstadoOrdenEnum, EstadoMiniOrden, TipoEmpleadoEnum…
│   ├── Exceptions/                 ← DomainException, StockInsuficienteException
│   └── Interfaces/                 ← IRepository, IUnitOfWork
│
├── Infrastructure/                 ← Implementaciones de infraestructura
│   ├── Data/
│   │   ├── ApplicationDbContext.cs ← DbContext con todos los DbSets
│   │   ├── Configurations/         ← Fluent API por entidad
│   │   ├── Migrations/             ← 15+ migraciones EF Core
│   │   └── Seeders/
│   │       └── DatabaseSeeder.cs   ← Datos iniciales (roles, usuarios, catálogos)
│   └── Services/
│       ├── JwtService.cs           ← Generación y validación de tokens JWT
│       ├── CurrentUserService.cs   ← Usuario autenticado del contexto HTTP
│       └── AuditoriaService.cs     ← Registro de auditoría
│
└── Console/                        ← CLI interactiva (Spectre.Console)
    └── Menus/                      ← Menús de gestión por consola
```

---

## Módulos y entidades

### Usuarios y seguridad

| Entidad | Descripción |
|---------|-------------|
| `Usuario` | Cuenta con email, contraseña (BCrypt) y roles |
| `Rol` | Admin, Mecánico, Recepcionista, JefeTaller, Cliente, MecanicoDiagnostico, MecanicoArea, JefeAlmacen, JefeBodega |
| `Empleado` | Perfil laboral del técnico (especialidad, TipoEmpleado, TipoServicioId) |
| `RefreshToken` | Tokens de renovación de sesión |
| `Permiso` / `RolPermiso` | Modelo de permisos granulares por rol |

### Clientes y vehículos

| Entidad | Descripción |
|---------|-------------|
| `Cliente` | Perfil con número auto-incremental, documentos, teléfonos, correos y direcciones |
| `Vehiculo` | Placa, marca, modelo, año, combustible, transmisión, kilometraje |
| `VehiculoPropietario` | Relación M:N cliente-vehículo (con fecha inicio/fin y estado activo) |

### Órdenes de servicio

| Entidad | Descripción |
|---------|-------------|
| `OrdenServicio` | Orden de trabajo con estados: Pendiente → Aprobada → EnProceso → Finalizada |
| `DetalleOrdenServicio` | Repuestos utilizados (cantidad y precio unitario) |
| `ManoObra` | Trabajo de mano de obra (descripción, costo, horas, mecánico) |
| `HistorialEstadoOrden` | Trazabilidad completa de cambios de estado |

### Presupuestos — Flujo M-J-C

| Entidad | Descripción |
|---------|-------------|
| `MiniOrden` | Presupuesto que sigue el flujo Mecánico → Jefe → Cliente |
| `MiniOrdenDetalle` | Repuestos cotizados con precio unitario |
| `MiniOrdenManoObra` | Mano de obra cotizada (tarifa/hora × horas estimadas) |
| `MiniOrdenHistorial` | Log de cada transición de estado con observaciones |
| `MiniOrdenAprobacion` | Registro de aprobaciones/rechazos por nivel |

### Inventario

| Entidad | Descripción |
|---------|-------------|
| `Repuesto` | Pieza o insumo (código, precio compra/venta, stock, TipoServicioId) |
| `CategoriaRepuesto` | Clasificación de repuestos |
| `Proveedor` | Datos del proveedor con contacto |
| `MovimientoInventario` | Registro de cada movimiento de stock |
| `EntradaInventario` / `SalidaInventario` | Entradas y salidas con motivo y referencia |
| `HistorialPrecioRepuesto` | Auditoría de cambios de precio de venta |

### Facturación y pagos

| Entidad | Descripción |
|---------|-------------|
| `Factura` | Documento fiscal consolidado: subtotal + IVA 19% − descuento = total |
| `Pago` | Registro de pagos recibidos (método, monto, referencia) |
| `SolicitudPago` | Intención de pago del cliente con token para pagos electrónicos |
| `MetodoPago` | Catálogo de métodos aceptados (Efectivo, Tarjeta, PSE, Nequi…) |

### Agenda

| Entidad | Descripción |
|---------|-------------|
| `Cita` | Agendamiento con cliente, vehículo, fecha y motivo |
| `EstadoCita` | Pendiente, Confirmada, Cancelada, Completada |

---

## Flujos de negocio

### Flujo M-J-C: Presupuesto → Orden de Servicio

```
[Mecánico]
  └─ Crea MiniOrden (Borrador)
       ├─ Selecciona tipo de servicio (PrecioBase se agrega como mano de obra)
       ├─ Agrega repuestos del inventario con precios reales
       └─ Envía al Jefe de Taller → (EnRevisionJefe)

[Jefe de Taller]
  └─ Revisa el presupuesto con desglose completo
       ├─ Aprueba → envía al cliente para revisión → (EnRevisionCliente)
       └─ Rechaza → devuelve con motivo al mecánico → (RechazadaJefe)

[Cliente] (desde portal "Mis Servicios Pendientes")
  └─ Ve el desglose: repuestos, mano de obra y totales
       ├─ Aprueba → se genera OrdenServicio automáticamente → (EnProceso)
       │    └─ Varios servicios del mismo vehículo = UNA sola OS consolidada
       └─ Rechaza → (RechazadaCliente)

[Mecánico / Jefe de Taller]
  └─ Ejecuta el trabajo y finaliza la orden → (Finalizada)
```

### Ciclo de vida de la Orden de Servicio

```
Pendiente ──► Aprobada ──► [Asignar Mecánico] ──► EnProceso ──► Finalizada
                                                                      │
                                                             Generar Factura
                                                             Consolidada
```

### Facturación consolidada

```
Múltiples OrdenesServicio (Finalizada + sin FacturaId)
  └─ POST /api/Facturas/consolidada { ClienteId }
       ├─ Suma repuestos + mano de obra de todas las órdenes
       ├─ Calcula: (Subtotal - Descuento) × 1.19 = Total con IVA
       ├─ Genera UNA Factura para el cliente
       └─ Marca todas las órdenes con el FacturaId
```

### Portal de pagos del cliente

```
Cliente selecciona método de pago:
  ├─ Efectivo → SolicitudPago (Pendiente)
  │     └─ Recepcionista confirma cobro → Factura.Pagada = true
  ├─ Tarjeta → procesa con datos de tarjeta → token CARD-XXXXXXXX-HHMMSS
  │     └─ Factura.Pagada = true automáticamente
  └─ PSE     → selecciona banco y documento → token PSE-XXXXXXXX-HHMMSS
        └─ Factura.Pagada = true automáticamente
```

---

## Autenticación y roles

### Políticas de autorización

| Política | Roles permitidos |
|----------|-----------------|
| `AdminOnly` | Admin |
| `StaffOnly` | Admin, Mecánico, MecanicoDiagnostico, MecanicoArea, Recepcionista, JefeTaller |
| `MecanicoOJefe` | Admin, JefeTaller, Mecánico, MecanicoDiagnostico, MecanicoArea |
| `JefeTallerOnly` | Admin, JefeTaller |
| `RecepcionOnly` | Admin, Recepcionista, JefeTaller |
| `AlmacenOnly` | Admin, JefeAlmacen, JefeBodega |
| `AlmacenOTaller` | Admin, JefeTaller, JefeAlmacen, JefeBodega, Mecánico, MecanicoArea |
| `ClienteOrAdmin` | Admin, Cliente, Recepcionista |
| `Reportes` | Admin, JefeTaller, JefeAlmacen, JefeBodega |

### Tokens JWT

- **Access Token:** 60 minutos de vigencia
- **Refresh Token:** renovación automática sin re-login
- **Claims:** `nameidentifier`, `email`, `name`, `role`

---

## Endpoints principales

### Autenticación
| Método | Ruta | Descripción |
|--------|------|-------------|
| POST | `/api/Auth/login` | Iniciar sesión → devuelve JWT + refreshToken |
| POST | `/api/Auth/register-cliente` | Registro de cliente desde portal público |
| POST | `/api/Auth/refresh-token` | Renovar token de acceso |
| POST | `/api/Auth/logout` | Revocar sesión |

### Clientes y Vehículos
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/Clientes?busqueda=` | Búsqueda insensible a mayúsculas/tildes |
| POST | `/api/Clientes` | Crear cliente + usuario vinculado automáticamente |
| GET | `/api/Vehiculos?ClienteId=` | Vehículos filtrados por propietario activo |

### Órdenes de Servicio
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/Ordenes?Estado=` | Listar con filtros (estado, cliente, vehículo, fechas) |
| POST | `/api/Ordenes/{id}/detalles` | Agregar repuesto a orden |
| DELETE | `/api/Ordenes/{id}/detalles/{did}` | Quitar repuesto |
| POST | `/api/Ordenes/{id}/manos-obra` | Agregar mano de obra |
| POST | `/api/Ordenes/{id}/aprobar` | Jefe aprueba orden |
| POST | `/api/Ordenes/{id}/asignar-mecanico` | Asignar mecánico (filtrado por especialidad) |
| POST | `/api/Ordenes/{id}/finalizar` | Finalizar orden |
| DELETE | `/api/Ordenes/{id}` | Soft delete (solo Admin/Jefe, no si tiene factura) |

### Presupuestos (M-J-C)
| Método | Ruta | Descripción |
|--------|------|-------------|
| POST | `/api/MiniOrdenes` | Crear presupuesto con repuestos y tipo de servicio |
| POST | `/api/MiniOrdenes/{id}/detalles` | Agregar repuesto al presupuesto |
| POST | `/api/MiniOrdenes/{id}/manos-obra` | Agregar mano de obra |
| POST | `/api/MiniOrdenes/{id}/enviar-revision` | Mecánico → Jefe |
| POST | `/api/MiniOrdenes/{id}/aprobacion-jefe` | Jefe aprueba → envía al cliente |
| POST | `/api/MiniOrdenes/{id}/aprobacion-cliente` | Cliente aprueba/rechaza |
| DELETE | `/api/MiniOrdenes/{id}` | Soft delete (mecánico, jefe o admin) |

### Inventario
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/Inventario/movimientos` | Historial de movimientos |
| POST | `/api/Inventario/entrada` | Registrar ingreso de mercancía |
| POST | `/api/Inventario/salida` | Registrar salida de stock |
| POST | `/api/Inventario/ajuste` | Ajuste por conteo físico |

### Facturación y Pagos
| Método | Ruta | Descripción |
|--------|------|-------------|
| POST | `/api/Facturas/consolidada` | Generar factura con todas las órdenes del cliente |
| GET | `/api/Facturas/ordenes-pendientes/{clienteId}` | Órdenes sin facturar con totales calculados |
| GET | `/api/Facturas/mis-facturas` | Facturas del cliente autenticado |
| POST | `/api/Facturas/iniciar-pago` | Cliente inicia pago (Efectivo/Tarjeta/PSE) |
| POST | `/api/Facturas/solicitudes-pago/{id}/confirmar` | Recepcionista confirma efectivo |
| GET | `/api/Facturas/solicitudes-pago?estado=Pendiente` | Ver cobros pendientes de confirmar |

### Dashboard y Catálogos
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/Dashboard/resumen` | Clientes, vehículos, órdenes activas, facturación mensual |
| GET | `/api/Dashboard/repuestos-criticos` | Repuestos bajo stock mínimo |
| GET | `/api/Catalogos/tipos-servicio` | Tipos de servicio con precio base |
| GET | `/api/Catalogos/categorias-repuesto` | Categorías de repuestos |
| PUT | `/api/Catalogos/configuraciones/{clave}` | Actualizar configuración del sistema |

> Documentación interactiva completa: **`http://localhost:5000/swagger`**

---

## Configuración

### `appsettings.Development.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=autotaller;Username=postgres;Password=TU_PASSWORD"
  },
  "JwtSettings": {
    "SecretKey": "AutoTallerManager_SuperSecret_JWT_Key_2024_Min32Chars!",
    "Issuer": "AutoTallerManager",
    "Audience": "AutoTallerManagerUsers",
    "ExpirationMinutes": 60
  }
}
```

### Rate Limiting

| Endpoint | Límite |
|----------|--------|
| POST /api/Auth/login | 10 req/min |
| POST /api/Auth/register | 5 req/min |
| GET (general) | 60 req/min |
| POST / PUT / DELETE | 30 req/min |

---

## Instalación y ejecución

### Prerrequisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9)
- [PostgreSQL 16+](https://www.postgresql.org/download/)
- EF Core CLI: `dotnet tool install --global dotnet-ef`

### Pasos

```bash
# 1. Clonar el repositorio
git clone https://github.com/TU_USUARIO/AutoTallerManager.git
cd AutoTallerManager

# 2. Configurar cadena de conexión
# Editar Api/appsettings.Development.json con tus credenciales de PostgreSQL

# 3. Aplicar migraciones (la base de datos se crea automáticamente)
dotnet ef database update --project Infrastructure --startup-project Api

# 4. Ejecutar la API
dotnet run --project Api

# API disponible en:  http://localhost:5000
# Swagger UI en:      http://localhost:5000/swagger
```

> Las migraciones y el seed de datos se aplican **automáticamente** al iniciar si no existen.

---

## Usuarios de prueba

| Rol | Email | Contraseña |
|-----|-------|------------|
| Administrador | admin@autotaller.com | Admin@123 |
| Jefe de Taller | jefe@autotaller.com | Jefe@123 |
| Mecánico | mecanico@autotaller.com | Mecanico@123 |
| Recepcionista | recepcion@autotaller.com | Recepcion@123 |
| Cliente | cliente@autotaller.com | Cliente@123 |
| Jefe Almacén | almacen@autotaller.com | Almacen@123 |
| Jefe Bodega | bodega@autotaller.com | Bodega@123 |
| Mec. Diagnóstico | diagnostico@autotaller.com | Diagnostico@123 |
| Mec. de Área | mecanicoArea@autotaller.com | MecArea@123 |

---

## Características destacadas

- **Búsqueda case-insensitive** con `ILike` de PostgreSQL en clientes, repuestos y vehículos
- **Validación vehículo-propietario**: el sistema impide asignar un vehículo que no pertenezca al cliente seleccionado (doble capa: frontend + backend)
- **Consolidación automática de presupuestos**: múltiples servicios aprobados para el mismo vehículo en el mismo día generan una sola Orden de Servicio
- **Precio base automático**: al crear un presupuesto con un tipo de servicio que tiene `PrecioBase`, este se agrega automáticamente como mano de obra
- **Filtro de mecánicos por especialidad**: al asignar mecánico a una orden, se muestran primero los especializados en ese tipo de servicio
- **Factura consolidada**: una factura agrupa todas las órdenes finalizadas de un cliente

---

*Desarrollado con ASP.NET Core 9 · Arquitectura Hexagonal · CQRS · PostgreSQL*

---

## Autores

| Nombre | Rol |
|--------|-----|
| **Kevin Santiago Sierra León** | Desarrollo Backend — Arquitectura, API, Base de datos |
| **Kevin Santiago Pinto Monsalve** | Desarrollo Frontend — Interfaz, UX, Integración |
