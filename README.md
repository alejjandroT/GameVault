# GameVault

## Tabla de contenido

- [Descripción](#descripción)
- [Stack tecnológico](#stack-tecnológico)
- [Arquitectura y capas](#arquitectura-y-capas)
- [Recorrido de una petición](#recorrido-de-una-petición)
- [Modelo de dominio](#modelo-de-dominio)
- [Dónde vive cada regla](#dónde-vive-cada-regla)
- [Modelo de datos](#modelo-de-datos)
- [Patrones y decisiones de diseño](#patrones-y-decisiones-de-diseño)
- [API y códigos de respuesta](#api-y-códigos-de-respuesta)
- [Formato de errores](#formato-de-errores)
- [Estructura del repositorio](#estructura-del-repositorio)
- [Crear la base de datos](#crear-la-base-de-datos)
- [Ejecutar la API y abrir Scalar](#ejecutar-la-api-y-abrir-scalar)
- [Pruebas](#pruebas)
- [Limitaciones conocidas y próximos pasos](#limitaciones-conocidas-y-próximos-pasos)

## Descripción

GameVault es una API para administrar un catálogo de videojuegos, clientes, órdenes de compra, pagos simulados y licencias digitales. Resuelve el flujo de compra de juegos digitales: valida disponibilidad y elegibilidad del cliente, conserva el precio al agregar el juego a una orden, procesa el pago y emite una licencia si el cobro es aprobado.

La aplicación usa una API HTTP pequeña y separa las reglas del dominio del almacenamiento y de la presentación para que puedan probarse sin base de datos ni servidor web.

## Stack tecnológico

- **.NET 10 / C#** para la API y las capas de aplicación.
- **ASP.NET Core Minimal APIs** con descubrimiento de endpoints `IEndpoint`.
- **SQLite** como base de datos relacional local, administrada con scripts SQL manuales.
- **Dapper** para SQL parametrizado y consultas de escritura y lectura, incluidas consultas con joins.
- **Scalar** para explorar la especificación OpenAPI en desarrollo.
- **xUnit** para las pruebas unitarias del dominio.
- **ProblemDetails** para respuestas de error HTTP con formato estándar.

## Arquitectura y capas

```text
Api [Minimal APIs, contratos HTTP, respuestas]
    |-- depende de --> Application [casos de uso, handlers, modelos y puertos]
    `-- depende de --> Infrastructure [repositorios SQLite/Dapper, gateways]
                                                 |-- implementa puertos de --> Application
                                                 `-- usa tipos de ----------> Domain
Application ---------------- depende de -----------> Domain
Domain [entidades, value objects, estados y reglas de negocio]
```

`Api` llama a casos de uso de `Application`; `Application` depende de `Domain` y declara abstracciones de persistencia. `Infrastructure` implementa esas abstracciones con SQLite/Dapper y también usa el dominio. `Domain` no depende de las otras capas. La API compone ambas ramas mediante inyección de dependencias.

## Recorrido de una petición

Ejemplo: `POST /api/ordenes`.

1. El endpoint Minimal API recibe `CrearOrdenRequest` y convierte los GUID recibidos a `ClientId` y `GameId`.
2. `CreateOrderHandler` verifica que la solicitud tenga juegos, busca el cliente, comprueba que esté activo y consulta cuántas órdenes pendientes vigentes tiene.
3. El handler carga cada juego. `Order.AddItem` exige que esté publicado y que no se repita; cada línea conserva el precio vigente en ese momento.
4. `OrderRepository` persiste la orden y sus líneas en una transacción SQLite.
5. El endpoint transforma la entidad a `OrdenResponse` y devuelve `201 Created`. Los errores de dominio se convierten en `ProblemDetails` con el código HTTP correspondiente.

En el pago, `PayOrderHandler` valida la vigencia y el total, llama al gateway simulado y, si se aprueba, guarda el pago, actualiza la orden y emite licencias dentro de una transacción.

## Modelo de dominio

### Entidades y value objects

- **Game**: catálogo de juegos, género, precio y estado. `Money` es un value object con un rango permitido de $0 a $500.
- **Client**: nombre, correo y estado activo. `Email` normaliza a minúsculas y valida el formato.
- **Order**: cliente, estado e ítems. `OrderItem` guarda el `GameId` y el precio unitario capturado al agregarlo.
- **Payment**: orden, monto, método y resultado del pago.
- **DigitalLicense**: licencia emitida para un juego de una orden y un cliente.
- **GameId**, **ClientId**, **OrderId**, **PaymentId** y **LicenseId**: tipos identificadores basados en `Guid`.

### Estados

| Modelo | Estados |
|---|---|
| `Game` | `Draft` → `Published` → `Discontinued` |
| `Order` | `Pending` → `Paid`, o `Pending` → `Cancelled` |
| `Payment` | `Approved` o `Rejected` |

Una orden `Pending` vence cuando han pasado más de 30 minutos desde su creación. No se puede pagar si venció. Un cliente puede tener como máximo tres órdenes `Pending` creadas dentro de los últimos 30 minutos.

## Dónde vive cada regla

| Regla | Capa responsable | Implementación o soporte |
|---|---|---|
| Título del juego requerido y de hasta 100 caracteres; género requerido | Domain | `Game.Create` / `Game.UpdateDetails` |
| Precio entre $0 y $500 | Domain y base de datos | `Money.Create`; restricciones `CHECK` en `Games.Price` y `OrderItems.UnitPrice` |
| Un juego nace en `Draft`; solo se publica desde `Draft` | Domain | `Game.Create` / `Game.Publish` |
| Solo se descontinúa desde `Published` | Domain | `Game.Discontinue` |
| No se cambia el precio de un juego descontinuado | Domain | `Game.UpdatePrice` |
| Títulos de juego únicos, sin distinguir mayúsculas/minúsculas | Application y base de datos | `CreateGameHandler` / `UpdateGameHandler`; `UNIQUE` sobre `Games.Title COLLATE NOCASE` |
| Nombre del cliente entre 2 y 100 caracteres | Domain | `Client.Create` / `Client.UpdateName` |
| Correo requerido, formato válido y normalización a minúsculas | Domain | `Email.Create` |
| Correo de cliente único | Application y base de datos | Handlers de cliente; restricción `UNIQUE` sobre `Clients.Email COLLATE NOCASE` |
| Activación y desactivación válidas | Domain | `Client.Activate` / `Client.Deactivate` |
| Solo clientes activos crean órdenes | Application | `CreateOrderHandler` |
| Una orden requiere al menos un juego | Application | `CreateOrderHandler` |
| Solo se agregan juegos publicados y no repetidos | Domain | `Order.AddItem`; además, `OrderItems` tiene unicidad por orden y juego |
| El precio de cada línea queda congelado al crear la orden | Domain | `Order.AddItem` copia `Game.Price` a `OrderItem.UnitPrice` |
| Cancelar solo una orden pendiente | Domain | `Order.Cancel` |
| Expiración después de 30 minutos y monto exacto para pagar | Domain | `Order.IsExpired` / `Payment.Create` |
| Máximo de tres órdenes pendientes vigentes por cliente | Application | `CreateOrderHandler` consulta `CountPendingAsync`; la base no impone atomicidad sobre esta regla |
| Token `tok_fail` rechaza el pago; otros tokens son aprobados por el simulador | Infrastructure | `FakePaymentGateway` |
| Pago aprobado cambia la orden a pagada y emite licencias | Application y Domain | `PayOrderHandler`, `Order.MarkAsPaid` y `DigitalLicense.Issue` |
| Integridad referencial, valores de estados, cantidades y claves únicas | Base de datos | `FOREIGN KEY`, `CHECK`, `UNIQUE` en `01-esquema.sql` |

## Modelo de datos

Las tablas están definidas en `database/01-esquema.sql`. Las fechas se guardan como texto UTC en formato `YYYY-MM-DD HH:MM:SS`.

| Tabla | Propósito y restricciones principales |
|---|---|
| `Games` | Catálogo; título único `NOCASE`, precio entre 0 y 500, estado entero 0/1/2. |
| `Clients` | Clientes; correo único `NOCASE`, `Active` limitado a 0/1. |
| `Orders` | Orden ligada a un cliente; estado limitado a 0/1/2. |
| `OrderItems` | Juegos y precio capturado por orden; FKs a `Orders` y `Games`, precio entre 0 y 500, combinación orden/juego única. |
| `Payments` | Pago asociado a una orden; importe no negativo, método 0/1 y estado 0/1. |
| `DigitalLicenses` | Licencia asociada a orden, cliente y juego; clave única y combinación orden/juego única. |

Hay índices para las relaciones y búsquedas frecuentes de órdenes, líneas de orden, pagos y licencias. `database/02-datos-iniciales.sql` agrega los juegos, clientes, órdenes y datos relacionados de ejemplo mediante `INSERT OR IGNORE`.

## Patrones y decisiones de diseño

- **Clean Architecture** y casos de uso organizados por feature.
- **DDD táctico**: entidades, value objects, errores de dominio, estados e identificadores tipados.
- **Result / Result<T>** para representar fallos de negocio sin excepciones como flujo normal.
- **Repository**: interfaces en `Application`, implementaciones en `Infrastructure`.
- **SQL parametrizado con Dapper**; los SQL no se construyen interpolando valores de entrada.
- **Modelos de lectura** específicos para listas y biblioteca, con joins y multi-mapping, separados de las entidades de escritura.
- **Inyección de dependencias**: el tiempo se abstrae con `TimeProvider`; auditoría obtiene el usuario actual mediante `IUsuarioActual`.
- **GUID como identificador** para las entidades y almacenamiento textual en SQLite.
- **Pasarela de pago simulada**: `tok_fail` fuerza rechazo; cualquier otro token es aprobado por la implementación actual. No procesa dinero real.
- **Transacción de pago aprobado**: escritura del pago, actualización de la orden y creación de licencias se confirman o revierten juntas.
- **SQLite** simplifica el despliegue local, pero permite un solo escritor a la vez; una carga concurrente puede esperar o encontrar contención de escritura.

## API y códigos de respuesta

Todas las rutas están bajo el prefijo `/api`. Los endpoints de creación que devuelven un recurso responden `201 Created`; las operaciones exitosas sin representación responden `204 No Content`, salvo la publicación y el pago, que devuelven un recurso.

| Verbo y ruta | Respuestas principales |
|---|---|
| `GET /api/juegos` | `200` |
| `GET /api/juegos/{id}` | `200`, `404` |
| `POST /api/juegos` | `201`, `400`, `409` |
| `PUT /api/juegos/{id}` | `204`, `400`, `404`, `409` |
| `DELETE /api/juegos/{id}` | `204`, `404`, `409` |
| `POST /api/juegos/{id}/publicar` | `200`, `404`, `409` |
| `GET /api/clientes` | `200` |
| `GET /api/clientes/{id}` | `200`, `404` |
| `POST /api/clientes` | `201`, `400`, `409` |
| `PUT /api/clientes/{id}` | `204`, `400`, `404`, `409` |
| `DELETE /api/clientes/{id}` | `204`, `404`, `409` |
| `GET /api/clientes/{id}/biblioteca?genero=` | `200`, `404` |
| `POST /api/ordenes` | `201`, `400`, `404`, `409` |
| `GET /api/ordenes?clienteId=&estado=` | `200`, `400` si el estado no es `Pending`, `Paid` o `Cancelled` |
| `GET /api/ordenes/{id}` | `200`, `404` |
| `POST /api/ordenes/{id}/cancelar` | `204`, `404`, `409` |
| `POST /api/ordenes/{id}/pagar` | `201`, `400`, `404`, `409` |

La secuencia numerada de peticiones reproducibles está en `src/GameVault.Api/GameVault.Api.http`. Para repetirla completa desde un estado conocido, vuelve a ejecutar los dos scripts de base de datos primero; el esquema restablece los datos y la secuencia usa nombres y correos fijos. Las órdenes de datos iniciales son antiguas respecto al reloj actual y sus estados no se actualizan automáticamente.

## Formato de errores

Los errores controlados se devuelven como `application/problem+json` (RFC 7807) con `status`, `title`, `detail` y `type`. La API agrega `errorCode` para identificar el error de dominio o validación. Por ejemplo, un juego duplicado o una orden vencida produce `409 Conflict`; un error de validación produce `400 Bad Request`.

Las excepciones no controladas se transforman en `500 Internal Server Error` con un `traceId`; el detalle interno de la excepción no se expone al cliente.

## Estructura del repositorio

```text
GameVault/
├── database/
│   ├── 01-esquema.sql
│   ├── 02-datos-iniciales.sql
│   └── gamevault.db                 # creado localmente por sqlite3
└── src/
    ├── GameVault.slnx
    ├── GameVault.Api/
    │   ├── Contracts/
    │   ├── Endpoints/
    │   ├── Extensions/
    │   ├── Middleware/
    │   ├── Properties/launchSettings.json
    │   ├── appsettings*.json
    │   └── GameVault.Api.http
    ├── GameVault.Application/
    │   ├── Abstractions/
    │   ├── Clients/
    │   ├── Games/
    │   ├── Licenses/
    │   ├── Orders/
    │   └── Payments/
    ├── GameVault.Domain/
    │   ├── Clients/
    │   ├── Common/
    │   ├── Games/
    │   ├── Licenses/
    │   ├── Orders/
    │   └── Payments/
    ├── GameVault.Infrastructure/
    │   ├── Games/
    │   ├── Migrations/
    │   ├── Persistence/Repositories/
    │   ├── Persistence/Registros/
    │   └── Servicios/
    └── GameVault.Domain.Tests/
        ├── Clients/
        ├── Common/
        ├── Games/
        ├── Orders/
        └── Payments/
```

## Crear la base de datos

Se requiere el ejecutable `sqlite3`. Desde la raíz del repositorio, ejecuta primero el esquema y luego los datos iniciales:

```sh
sqlite3 database/gamevault.db < database/01-esquema.sql
sqlite3 database/gamevault.db < database/02-datos-iniciales.sql
```

**Advertencia:** `01-esquema.sql` elimina y vuelve a crear las tablas. La cadena de conexión de `src/GameVault.Api/appsettings.json` apunta a `../../database/gamevault.db`; ejecuta la API desde `src/GameVault.Api` para que esa ruta relativa llegue al archivo creado en `database/`.

## Ejecutar la API y abrir Scalar

Con la base de datos creada, desde la raíz:

```sh
cd src/GameVault.Api
dotnet run --launch-profile http
```

El perfil `http` de `Properties/launchSettings.json` escucha en `http://localhost:5200`. En desarrollo, abre Scalar en [http://localhost:5200/scalar](http://localhost:5200/scalar); la especificación OpenAPI se publica en `/openapi/v1.json`.

Para compilar la solución y ejecutar las pruebas desde `src/`:

```sh
cd src
dotnet build GameVault.slnx
dotnet test GameVault.slnx
```

## Pruebas

`GameVault.Domain.Tests` usa xUnit y solo referencia `GameVault.Domain`. Las pruebas ejercitan reglas de entidades, value objects, igualdad de entidades y límites temporales usando fechas fijas. No requieren SQLite, HTTP ni la API.

## Limitaciones conocidas y próximos pasos

- La comprobación de tres órdenes pendientes y la inserción ocurren en operaciones separadas. Dos peticiones simultáneas pueden observar un conteo menor que tres y crear órdenes que superen el límite; la base de datos no impone actualmente esa regla.
- SQLite admite un solo escritor concurrente, por lo que la concurrencia de escritura es limitada.
- El gateway de pago es una simulación y el usuario de auditoría está fijado a `sistema`; no hay autenticación de usuarios.
- Los siguientes pasos posibles son incorporar JWT, integrar Stripe, migrar a PostgreSQL y permitir regalar juegos.
