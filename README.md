# TicketMaster

> Modern etkinlik ve bilet yönetimi için geliştirilmiş, .NET 10 tabanlı RESTful Web API.
> A modern .NET 10 RESTful Web API for event and ticket-type management.

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-10.0-512BD4?logo=dotnet&logoColor=white)](https://learn.microsoft.com/aspnet/core)
[![Entity Framework Core](https://img.shields.io/badge/EF%20Core-10.0-512BD4?logo=dotnet&logoColor=white)](https://learn.microsoft.com/ef/core/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-supported-CC2927?logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)

## İçindekiler | Contents

- [Proje Hakkında | About](#proje-hakkında--about)
- [Özellikler | Features](#özellikler--features)
- [Mimari | Architecture](#mimari--architecture)
- [Teknolojiler | Tech Stack](#teknolojiler--tech-stack)
- [Proje Yapısı | Project Structure](#proje-yapısı--project-structure)
- [Gereksinimler | Prerequisites](#gereksinimler--prerequisites)
- [Kurulum | Installation](#kurulum--installation)
- [Veritabanı | Database](#veritabanı--database)
- [Yapılandırma | Configuration](#yapılandırma--configuration)
- [Çalıştırma | Running](#çalıştırma--running)
- [API](#api)
- [Kimlik Doğrulama | Authentication](#kimlik-doğrulama--authentication)
- [Güvenlik Notları | Security Notes](#güvenlik-notları--security-notes)

---

## Proje Hakkında | About

**TicketMaster**, etkinlikleri ve etkinliklere ait bilet türlerini yönetmek için hazırlanmış bir backend projesidir. Uygulama; kullanıcı kaydı, JWT tabanlı giriş, rol bazlı yetkilendirme, etkinlik CRUD işlemleri ve bilet türü yönetimi sunar.

**TicketMaster** is a backend application for managing events and their ticket types. It provides user registration, JWT-based login, role-based authorization, event CRUD operations, and ticket-type management.

Proje, bağımlılıkların dış katmanlardan iç katmanlara doğru aktığı **Clean Architecture** yaklaşımıyla geliştirilmiştir. CQRS yaklaşımı, MediatR, Entity Framework Core ve ASP.NET Core Identity bu mimarinin temel parçalarını oluşturur.

The project follows **Clean Architecture**, where dependencies point from the outer layers toward the inner layers. CQRS-style request handling, MediatR, Entity Framework Core, and ASP.NET Core Identity are key parts of the implementation.

> **Durum | Status:** Proje geliştirme aşamasındadır. Sipariş ve bilet satın alma akışları için domain altyapısı bulunsa da mevcut HTTP API’de bu işlemler henüz yayınlanmamıştır.

## Özellikler | Features

- Kullanıcı kaydı ve girişi / User registration and login
- JWT access token üretimi / JWT access-token generation
- ASP.NET Core Identity ile kullanıcı ve rol yönetimi / User and role management with ASP.NET Core Identity
- `Admin` ve `User` rollerinin başlangıçta seed edilmesi / Startup seeding of `Admin` and `User` roles
- Etkinlik oluşturma, listeleme, detay görüntüleme, güncelleme ve silme / Event create, list, detail, update, and delete
- Bilet türü oluşturma, listeleme, detay görüntüleme, güncelleme ve silme / Ticket-type create, list, detail, update, and delete
- FluentValidation ile request doğrulama / Request validation with FluentValidation
- MediatR ile application katmanı istek yönetimi / Application request handling with MediatR
- Entity Framework Core ve SQL Server ile veri erişimi / Data access with EF Core and SQL Server
- Soft delete ve audit alanları / Soft delete and audit metadata
- Scalar ile interaktif OpenAPI dokümantasyonu / Interactive OpenAPI documentation with Scalar
- ASP.NET Core rate limiting / ASP.NET Core rate limiting
- .NET Aspire AppHost ile çalıştırma desteği / .NET Aspire AppHost support
- OpenTelemetry ve service defaults altyapısı / OpenTelemetry and service-defaults infrastructure

## Mimari | Architecture

```text
						 +----------------------+
						 |  TicketMaster.AppHost |
						 |     .NET Aspire       |
						 +----------+-----------+
									|
									v
+-------------+       +-------------+--------------+
|   Clients   | ----> |       WebAPI              |
| Web / Mobile|       | Controllers, Middleware,  |
| API Clients |       | OpenAPI, Rate Limiting    |
+-------------+       +-------------+--------------+
									|
									v
						 +----------+-----------+
						 |   Application         |
						 | CQRS, MediatR,        |
						 | Validation, DTOs      |
						 +----------+-----------+
									|
									v
						 +----------+-----------+
						 |   Domain              |
						 | Entities, Value       |
						 | Objects, Contracts    |
						 +----------+-----------+
									^
									|
						 +----------+-----------+
						 |   Infrastructure     |
						 | EF Core, SQL Server,  |
						 | Identity, JWT, Repos  |
						 +----------------------+
```

### Clean Architecture katmanları | Clean Architecture layers

- **WebAPI:** HTTP endpoint'leri, middleware, CORS, rate limiting ve API sunumu.
- **Application:** Use-case'ler, MediatR command/query'leri, DTO'lar ve FluentValidation kuralları.
- **Domain:** İş varlıkları, domain sözleşmeleri ve repository arayüzleri.
- **Infrastructure:** EF Core `AppDbContext`, SQL Server, Identity, JWT, repository implementasyonları ve migration'lar.
- **ServiceDefaults:** .NET Aspire servis varsayılanları, health check, service discovery ve telemetry yapılandırması.
- **AppHost:** .NET Aspire uygulama orkestrasyonu.

## Teknolojiler | Tech Stack

| Alan | Teknoloji |
| --- | --- |
| Runtime | .NET 10 |
| Web framework | ASP.NET Core 10 |
| Language | C# |
| Database | Microsoft SQL Server / LocalDB |
| ORM | Entity Framework Core 10 |
| Authentication | ASP.NET Core Identity + JWT Bearer |
| Application pattern | CQRS-style commands/queries + MediatR |
| Validation | FluentValidation |
| Mapping | Mapster |
| API documentation | Microsoft OpenAPI + Scalar |
| Resilience/observability | .NET Aspire ServiceDefaults + OpenTelemetry |
| Container/orchestration support | .NET Aspire AppHost |


## Proje Yapısı | Project Structure

```text
TicketMaster/
├── src/
│   ├── WebAPI/                  # HTTP API, controllers and middleware
│   ├── TicketMaster.Application/ # Commands, queries, validators and DTOs
│   ├── TicketMaster.Domain/      # Domain entities and repository contracts
│   ├── TicketMaster.Infrastructure/# EF Core, Identity, JWT and repositories
│   ├── TicketMaster.ServiceDefaults/# Aspire defaults and observability
│   └── TicketMaster.AppHost/    # .NET Aspire application host
├── TicketMaster.slnx
└── README.md
```

## Gereksinimler | Prerequisites

- Visual Studio Community 2026 veya güncel bir .NET IDE
- .NET 10 SDK
- SQL Server veya SQL Server Express LocalDB
- Git
- İsteğe bağlı: .NET Aspire workload/tools

.NET sürümünü kontrol etmek için / To check the installed .NET version:

```powershell
dotnet --version
```

LocalDB kontrolü / Check LocalDB:

```powershell
sqllocaldb info
sqllocaldb info MSSQLLocalDB
```

LocalDB örneğini başlatmak için / Start the LocalDB instance:

```powershell
sqllocaldb start MSSQLLocalDB
```

## Kurulum | Installation

### 1. Repoyu klonlayın | Clone the repository

```powershell
git clone https://github.com/<your-username>/TicketMaster.git
cd TicketMaster
```

### 2. Bağımlılıkları geri yükleyin | Restore dependencies

```powershell
dotnet restore TicketMaster.slnx
```

### 3. Geliştirme ayarlarını oluşturun | Configure development settings

`src/WebAPI/appsettings.json` içindeki connection string ve JWT secret alanları bilerek boştur. Gerçek değerleri dosyaya yazmak yerine User Secrets, environment variable veya güvenli bir secret manager üzerinden sağlayın. Projede `UserSecretsId` zaten tanımlıdır.

The connection string and JWT secret fields in `src/WebAPI/appsettings.json` are intentionally empty. Provide real values through User Secrets, environment variables, or a secure secret manager. `UserSecretsId` is already configured in the project.

### 4. Veritabanını oluşturun | Create the database

```powershell
dotnet ef database update `
  --project src/TicketMaster.Infrastructure/TicketMaster.Infrastructure.csproj `
  --startup-project src/WebAPI/WebAPI.csproj
```

EF CLI kurulu değilse / If the EF CLI is not installed:

```powershell
dotnet tool install --global dotnet-ef
```

## Veritabanı | Database

Uygulama `Microsoft.EntityFrameworkCore.SqlServer` kullanır ve Identity tabloları ile uygulama tablolarını aynı `AppDbContext` içinde yönetir.

The application uses `Microsoft.EntityFrameworkCore.SqlServer` and manages Identity and application tables through the same `AppDbContext`.

### Varsayılan yerel bağlantı | Default local connection

Yerel geliştirme için proje LocalDB kullanacak şekilde yapılandırılmıştır:

```text
Server=(localdb)\MSSQLLocalDB;Database=TicketMasterDb;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=False
```

`TrustServerCertificate=False` kullanıldığı için SQL Server sertifikasının işletim sistemi tarafından güvenilir olması gerekir. Üretimde şifrelemeyi kapatmayın; geçerli ve güvenilir bir SQL Server sertifikası yapılandırın.

Because `TrustServerCertificate=False` is used, the SQL Server certificate must be trusted by the operating system. Do not disable encryption in production; configure a valid, trusted SQL Server certificate instead.

### Yeni migration oluşturma | Create a migration

```powershell
dotnet ef migrations add MigrationName `
  --project src/TicketMaster.Infrastructure/TicketMaster.Infrastructure.csproj `
  --startup-project src/WebAPI/WebAPI.csproj
```

### Migration uygulama | Apply migrations

```powershell
dotnet ef database update `
  --project src/TicketMaster.Infrastructure/TicketMaster.Infrastructure.csproj `
  --startup-project src/WebAPI/WebAPI.csproj
```

Uygulama başlangıçta `Admin` ve `User` rollerini seed etmeyi dener. Bu nedenle API başlatılmadan önce veritabanı bağlantısının çalışıyor olması gerekir.

On startup, the application seeds the `Admin` and `User` roles. The database connection must therefore be available before the API starts.

## Yapılandırma | Configuration

Temel ayarlar `src/WebAPI/appsettings.json` dosyasındadır. Public repository için hassas değerleri environment variable, User Secrets veya güvenli bir secret manager üzerinden sağlayın.

The base settings are in `src/WebAPI/appsettings.json`. For a public repository, provide sensitive values through environment variables, User Secrets, or a secure secret manager.

Örnek User Secrets kullanımı / Example User Secrets setup:

```powershell
dotnet user-secrets set "ConnectionStrings:SqlServer" "<your-connection-string>" --project src/WebAPI/WebAPI.csproj
dotnet user-secrets set "JwtOptions:SecretKey" "<long-random-secret>" --project src/WebAPI/WebAPI.csproj
```

Temel JWT ayarları / Core JWT settings:

```json
{
  "JwtOptions": {
	"Issuer": "TicketMaster",
	"Audience": "TicketMaster",
	"SecretKey": "<store-this-outside-source-control>"
  }
}
```

JWT secret için güçlü, rastgele ve production ortamında ayrı bir değer kullanın. Public repository'ye yanlışlıkla secret gönderildiyse hemen rotate edin.

Use a strong, random, environment-specific JWT secret. If a secret was accidentally committed to a public repository, rotate it immediately.

## Çalıştırma | Running

### WebAPI projesini doğrudan çalıştırma | Run WebAPI directly

```powershell
dotnet run --project src/WebAPI/WebAPI.csproj
```

Varsayılan geliştirme adresleri / Default development URLs:

- HTTP: `http://localhost:5278`
- HTTPS: `https://localhost:7021`
- Scalar UI: `https://localhost:7021/scalar/v1` veya `http://localhost:5278/scalar/v1`

### .NET Aspire AppHost ile çalıştırma | Run with .NET Aspire AppHost

```powershell
dotnet run --project src/TicketMaster.AppHost/TicketMaster.AppHost.csproj
```

AppHost, `WebAPI` projesini Aspire üzerinden başlatır ve geliştirme dashboard'ını sağlar. AppHost şu anda SQL Server kaynağı provision etmez; erişilebilir bir SQL Server veya LocalDB örneği ayrıca gereklidir.

The AppHost starts the `WebAPI` project through Aspire and provides the development dashboard. It does not provision a SQL Server resource at this time, so a reachable SQL Server or LocalDB instance is still required.

## API

API istekleri `/api` prefix'i altında yayınlanır. Tüm endpoint'ler Scalar/OpenAPI arayüzünden de incelenebilir.

API requests use the `/api` prefix. All endpoints can also be explored through the Scalar/OpenAPI UI.

### Kimlik doğrulama | Authentication endpoints

| Method | Endpoint | Auth | Açıklama / Description |
| --- | --- | --- | --- |
| `POST` | `/api/Auth/register` | Public | Kullanıcı kaydı / Register a user |
| `POST` | `/api/Auth/login` | Public | JWT token al / Obtain a JWT token |

Kayıt örneği / Registration example:

```json
{
  "firstName": "Ada",
  "lastName": "Lovelace",
  "email": "ada@example.com",
  "password": "StrongPassword123!",
  "phone": "+1 555 0100"
}
```

Giriş örneği / Login example:

```json
{
  "email": "ada@example.com",
  "password": "StrongPassword123!"
}
```

Girişten dönen `accessToken` değerini korumalı endpoint'lerde kullanın / Use the returned `accessToken` for protected endpoints:

```text
Authorization: Bearer <access-token>
```

### Etkinlik endpoint'leri | Event endpoints

| Method | Endpoint | Auth | Açıklama / Description |
| --- | --- | --- | --- |
| `GET` | `/api/Events` | Public | Etkinlikleri listele / List events |
| `GET` | `/api/Events/{id}` | Public | Etkinlik detayı / Get event details |
| `POST` | `/api/Events` | `Admin` | Etkinlik oluştur / Create an event |
| `PUT` | `/api/Events/{id}` | `Admin` | Etkinlik güncelle / Update an event |
| `DELETE` | `/api/Events/{id}` | `Admin` | Etkinlik sil / Delete an event |

Etkinlik oluşturma/güncelleme örneği / Event create/update example:

```json
{
  "title": "Summer Music Festival",
  "type": "Festival",
  "date": "2027-07-15",
  "time": "20:00:00",
  "description": "An open-air music festival.",
  "capacity": 5000,
  "addressDto": {
	"country": "Türkiye",
	"city": "Istanbul",
	"location": "Kadıköy",
	"fullAddress": "Example Street No: 10"
  }
}
```

`type` değeri domain enum'ına göre `Konser`, `Spor`, `Tiyatro`, `Sinema` veya `Festival` olmalıdır.

The `type` value must match the domain enum: `Konser`, `Spor`, `Tiyatro`, `Sinema`, or `Festival`.

### Bilet türü endpoint'leri | Ticket-type endpoints

| Method | Endpoint | Auth | Açıklama / Description |
| --- | --- | --- | --- |
| `GET` | `/api/TicketTypes` | Public | Bilet türlerini listele / List ticket types |
| `GET` | `/api/TicketTypes/{id}` | Public | Bilet türü detayı / Get ticket-type details |
| `POST` | `/api/TicketTypes` | `Admin` | Bilet türü oluştur / Create a ticket type |
| `PUT` | `/api/TicketTypes/{id}` | `Admin` | Bilet türü güncelle / Update a ticket type |
| `DELETE` | `/api/TicketTypes/{id}` | `Admin` | Bilet türü sil / Delete a ticket type |

Bilet türü oluşturma örneği / Ticket-type create example:

```json
{
  "name": "General Admission",
  "price": 750.00,
  "eventId": "00000000-0000-0000-0000-000000000000",
  "totalQuantity": 1000
}
```

Bilet türlerinin toplam miktarı ilgili etkinliğin kapasitesini aşamaz. Satılmış bilet miktarının altına düşecek bir güncelleme yapılamaz.

Ticket-type quantities cannot exceed the related event capacity. An update cannot reduce the quantity below the number of tickets already sold.

## Kimlik Doğrulama | Authentication

- Yeni kullanıcılar varsayılan olarak `User` rolüyle oluşturulur.
- Etkinlik ve bilet türü yazma/silme işlemleri yalnızca `Admin` rolüne açıktır.
- Token üretiminde `Issuer`, `Audience`, `SecretKey` ve token lifetime ayarları `JwtOptions` üzerinden okunur.
- Başlangıçta `Admin` ve `User` rolleri otomatik seed edilir.

- New users are assigned the `User` role by default.
- Event and ticket-type write/delete operations require the `Admin` role.
- Token validation uses `Issuer`, `Audience`, `SecretKey`, and lifetime settings from `JwtOptions`.
- The `Admin` and `User` roles are seeded during startup.


## Güvenlik Notları | Security Notes

- `appsettings.json` içinde gerçek JWT secret veya production connection string tutmayın.
- Public repository'ye gönderilmiş bir secret varsa yalnızca dosyadan silmek yeterli değildir; secret'ı rotate edin.
- Production ortamında `Encrypt=True;TrustServerCertificate=False` kullanın ve SQL Server için işletim sistemi tarafından güvenilen bir sertifika yapılandırın.
- CORS ayarları production için daraltılmalıdır; mevcut API geliştirme amacıyla tüm origin'lere izin vermektedir.
- Production ortamında HTTPS kullanın.
- Kullanıcı parolalarını kendiniz hash'lemeyin; ASP.NET Core Identity'nin password hasher'ını kullanın.

- Do not store real JWT secrets or production connection strings in `appsettings.json`.
- If a secret was pushed to a public repository, deleting the file is not enough; rotate the secret.
- Use `Encrypt=True;TrustServerCertificate=False` in production with a certificate trusted by the operating system.
- Restrict CORS origins for production; the current API configuration allows any origin for development.
- Use HTTPS in production.
- Do not hash passwords manually; rely on ASP.NET Core Identity's password hasher.

