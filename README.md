# 🔧 AutoTallerManager

Sistema empresarial de gestión integral para talleres automotrices modernos.

![.NET](https://img.shields.io/badge/.NET-9.0-purple)
![EF Core](https://img.shields.io/badge/EF%20Core-9.0-blue)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-blue)
![Architecture](https://img.shields.io/badge/Architecture-Hexagonal-green)

---

## 📋 Descripción

**AutoTallerManager** es una solución empresarial completa para la administración de talleres automotrices. Implementa Arquitectura Hexagonal (Ports & Adapters), CQRS, DDD y Clean Architecture sobre ASP.NET Core 9.

---

## 🏗️ Arquitectura

```
AutoTallerManager/
├── src/
│   ├── AutoTallerManager.API           → Controladores, Middleware, Swagger
│   ├── AutoTallerManager.Application   → CQRS, Commands, Queries, DTOs, Validators
│   ├── AutoTallerManager.Domain        → Entidades, Interfaces, Excepciones, Enums
│   ├── AutoTallerManager.Infrastructure→ EF Core, Repositorios, Servicios JWT
│   └── AutoTallerManager.Shared        → Modelos comunes, Extensions, Constants
├── console/
│   └── AutoTallerManager.ConsoleClient → Spectre.Console (HTTP only)
├── tests/
│   ├── AutoTallerManager.UnitTests     → xUnit, Moq, FluentAssertions
│   └── AutoTallerManager.IntegrationTests
└── docker/
    ├── Dockerfile.api
    ├── Dockerfile.console
    └── docker-compose.yml
```

---

## 🚀 Inicio Rápido

### Con Docker (Recomendado)

```bash
cd docker
docker-compose up -d
```

- **API:** http://localhost:5000
- **Swagger:** http://localhost:5000/swagger
- **PostgreSQL:** localhost:5432

### Sin Docker

**Requisitos:**
- .NET 9 SDK
- PostgreSQL 16+

**1. Configurar base de datos:**

```bash
# Editar cadena de conexión
# src/AutoTallerManager.API/appsettings.Development.json
```

**2. Aplicar migraciones:**

```bash
cd src/AutoTallerManager.API
dotnet ef database update --project ../AutoTallerManager.Infrastructure
```

**3. Ejecutar API:**

```bash
dotnet run --project src/AutoTallerManager.API
```

**4. Ejecutar consola (en otra terminal):**

```bash
dotnet run --project console/AutoTallerManager.ConsoleClient
```

---

## 🔐 Credenciales por Defecto

| Usuario | Contraseña | Rol |
|---------|-----------|-----|
| admin@autotaller.com | Admin@123 | Admin |

---

## 📡 Endpoints Principales

### Autenticación
```
POST /api/auth/login
POST /api/auth/register
POST /api/auth/refresh-token
POST /api/auth/revoke-token
```

### Módulos CRUD
```
/api/clientes      - Gestión de clientes
/api/vehiculos     - Gestión de vehículos
/api/citas         - Agenda y citas
/api/ordenes       - Órdenes de servicio
/api/repuestos     - Catálogo de repuestos
/api/inventario    - Movimientos de inventario
/api/facturas      - Facturación
/api/empleados     - Personal
/api/auditorias    - Log de auditoría
/api/dashboard     - Métricas y reportes
```

---

## 🛡️ Seguridad

- **JWT Bearer Authentication** con Refresh Tokens
- **Roles:** Admin, Mecánico, Recepcionista, Cliente
- **Rate Limiting:** 30-60 req/min por endpoint
- **Soft Delete** global
- **Auditoría automática** en todas las entidades

---

## 🧪 Tests

```bash
# Unit Tests
dotnet test tests/AutoTallerManager.UnitTests

# Integration Tests
dotnet test tests/AutoTallerManager.IntegrationTests

# Todos
dotnet test
```

---

## 🐳 Variables de Entorno (Docker)

| Variable | Valor por defecto |
|----------|------------------|
| `ConnectionStrings__DefaultConnection` | Host=postgres;Database=AutoTallerManager;Username=postgres;Password=Admin@123 |
| `JwtSettings__SecretKey` | SuperSecretKey... |
| `JwtSettings__ExpirationMinutes` | 60 |

---

## 📦 Stack Tecnológico

| Tecnología | Versión |
|-----------|---------|
| ASP.NET Core | 9.0 |
| Entity Framework Core | 9.0 |
| PostgreSQL | 16 |
| MediatR (CQRS) | 12.x |
| AutoMapper | 13.x |
| FluentValidation | 11.x |
| Serilog | 8.x |
| Spectre.Console | 0.49.x |
| xUnit | 2.9.x |

---

## 👤 Autor

Generado con arquitectura empresarial profesional siguiendo estándares de:
- Clean Architecture
- Domain Driven Design
- SOLID Principles
- Hexagonal Architecture (Ports & Adapters)

---

## 📄 Licencia

MIT License
