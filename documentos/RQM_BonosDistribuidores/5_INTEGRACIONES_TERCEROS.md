# 5. INTEGRACIONES CON TERCEROS - Bonos Distribuidores

## Status: ? PENDIENTE DEFINICIÓN

---

## ?? Secciones a Documentar

### 5.1 Proveedores Externos a Integrar

```
Proveedor 1: ?
  [ ] Nombre del servicio/API
  [ ] URL base: ?
  [ ] Documentación: ?
  [ ] Contacto: ?

Proveedor 2: [Por completar]

Proveedor N: [Por completar]
```

### 5.2 Integración con Proveedor 1

#### 5.2.1 Información General
```
Nombre: ?
Tipo de API: [ ] REST [ ] GraphQL [ ] SOAP [ ] Otros
Protocolo: [ ] HTTP [ ] HTTPS (requerido)
Versión API: ?
Documentación Oficial: [Link]
```

#### 5.2.2 Autenticación
```
Tipo: [ ] API Key [ ] OAuth2 [ ] mTLS [ ] Basic Auth [ ] JWT [ ] Otros
Credenciales/Secrets:
  - [ ] API_KEY = ?
  - [ ] CLIENT_ID = ?
  - [ ] CLIENT_SECRET = ?
  - [ ] USERNAME = ?
  - [ ] PASSWORD = ?
  - [ ] CERTIFICATE = ?

Dónde se almacenan:
  [ ] appsettings.json
  [ ] Azure Key Vault
  [ ] User Secrets (.NET)
  [ ] Variables de entorno
  [ ] Otros: ?

Expiración de credenciales:
  [ ] Sí (cada X meses)
  [ ] No
  [ ] Depende de: ?
```

#### 5.2.3 Endpoints Utilizados
```
Endpoint 1: GET /api/v1/bonuses
  Descripción: Obtener bonos desde tercero
  Parámetros: ?
  Headers: ?
  Response: ?
  Error handling: ?

Endpoint 2: POST /api/v1/apply-bonus
  Descripción: Aplicar bono en sistema tercero
  Body: ?
  Response: ?

Endpoint 3: PUT /api/v1/bonuses/{id}
  Descripción: Actualizar bono

Endpoint N: [Por completar]
```

#### 5.2.4 Rate Limiting
```
[ ] ¿Existe límite de llamadas?
[ ] Límite: X requests por Y segundos
[ ] Estrategia si se excede: [ ] Queue [ ] Retry [ ] Error [ ] Otros
[ ] Timeout por request: ? segundos
```

#### 5.2.5 Comunicación

```
Tipo: [ ] Síncrono [ ] Asíncrono [ ] Ambos

Si es Síncrono:
  [ ] Llamadas directas desde servicio
  [ ] HttpClient + Polly (reintentos)
  [ ] Timeout: ? segundos
  [ ] Reintentos: ? intentos
  [ ] Backoff: [ ] Exponencial [ ] Lineal [ ] Fijo

Si es Asíncrono:
  [ ] Publicación de eventos en RabbitMQ
  [ ] Worker que consume y llama al tercero
  [ ] Webhook para recibir respuesta
  [ ] Dead Letter Queue para errores
```

#### 5.2.6 Modelado de Datos
```csharp
Cliente HTTP/Refit:
  [ ] IProviderXApi.cs (interfaz Refit)

DTOs de Request:
  [ ] CreateBonusRequest
  [ ] ApplyBonusRequest
  [ ] Otros: ?

DTOs de Response:
  [ ] BonusResponse
  [ ] BonusApplicationResponse
  [ ] ErrorResponse
  [ ] Otros: ?

Ubicación: 
  - Interfaces: Aldebaran.Infraestructure.Core/ExternalApis/
  - DTOs: Aldebaran.Infraestructure.Core/ExternalApis/Models/
```

#### 5.2.7 Manejo de Errores
```
[ ] Códigos HTTP esperados: 200, 201, 400, 401, 404, 429, 500, etc.

Estrategia por tipo de error:
  [ ] 4xx (Error del cliente)
      - Acción: [ ] Log + Alert [ ] Retry [ ] Fallar inmediato

  [ ] 5xx (Error del servidor)
      - Acción: [ ] Retry con backoff [ ] Queue para reintentar [ ] Fallar

  [ ] Timeout
      - Acción: [ ] Retry [ ] Queue [ ] Fallar

  [ ] Connection refused
      - Acción: [ ] Retry [ ] Usar fallback [ ] Fallar

Fallback strategy (si aplica):
  [ ] Usar cache
  [ ] Usar datos locales
  [ ] Informar al usuario
  [ ] Otros: ?
```

#### 5.2.8 Logging y Monitoreo
```
[ ] Log de cada llamada:
    - Request: URL, Headers (sin secrets), Body
    - Response: Status, Headers, Body
    - Tiempo de respuesta
    - Errores: Exception + StackTrace

[ ] Métricas a recopilar:
    - Cantidad de llamadas por hora
    - Promedio de tiempo de respuesta
    - Tasa de error
    - Disponibilidad del servicio

[ ] Alertas si:
    - Tasa de error > X%
    - Tiempo promedio > X ms
    - Servicio no responde
```

#### 5.2.9 Pruebas
```
[ ] Unitarias: Mock del cliente Refit
[ ] Integración: Contra sandbox/staging del tercero
[ ] Carga: Simular X requests/segundo
[ ] Fallback: Simular errores y timeouts
```

### 5.3 Integración con Proveedor 2

#### [Repetir 5.2 para cada proveedor]

---

## ?? Configuración en appsettings.json

```json
{
  "ExternalServices": {
    "Provider1": {
      "BaseUrl": "https://api.provider1.com",
      "ApiKey": "${PROVIDER1_API_KEY}",
      "ClientId": "${PROVIDER1_CLIENT_ID}",
      "ClientSecret": "${PROVIDER1_CLIENT_SECRET}",
      "Timeout": 30,
      "RetryCount": 3,
      "RetryDelay": 1000,
      "EnableLogging": true
    },
    "Provider2": {
      // [Similar]
    }
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "VirtualHost": "/",
    "Exchanges": [
      {
        "Name": "bonuses.exchange",
        "Type": "topic",
        "Durable": true
      }
    ],
    "Queues": [
      {
        "Name": "bonuses.apply.queue",
        "Exchange": "bonuses.exchange",
        "RoutingKey": "bonus.apply"
      }
    ]
  }
}
```

---

## ?? Implementación de Cliente HTTP

### Opción A: Refit (Recomendado)
```csharp
// Ubicación: Aldebaran.Infraestructure.Core/ExternalApis/IProvider1Api.cs

[Headers("Authorization: Bearer")]
public interface IProvider1Api
{
    [Get("/api/v1/bonuses")]
    Task<BonusesResponse> GetBonusesAsync([Query] int page, [Query] int pageSize);

    [Post("/api/v1/apply-bonus")]
    Task<ApplyBonusResponse> ApplyBonusAsync([Body] ApplyBonusRequest request);
}

// Registro en Program.cs (Extension)
services.AddRefitClient<IProvider1Api>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(configuration["ExternalServices:Provider1:BaseUrl"]))
    .AddTransientHttpErrorPolicy(builder =>
        builder.WaitAndRetryAsync(3, retryAttempt =>
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));
```

### Opción B: HttpClient + Polly
```csharp
// [Alternativa si Refit no aplica]
```

---

## ?? Flujos de Sincronización

### Flujo 1: Sincronización Periódica de Bonos
```
[A definir]

1. Scheduler dispara cada X horas
2. Llama a Proveedor1 para obtener bonos
3. Compara con BD local
4. Inserta/actualiza registros
5. Publica evento BonusSyncedEvent en RabbitMQ
6. Log de operación
```

### Flujo 2: Aplicación de Bono en Tiempo Real
```
[A definir]

1. Usuario aplica bono en UI
2. Sistema valida bono localmente
3. Llama a Proveedor1 para confirmar
4. Si OK: Registra aplicación
5. Si Error: Reintenta con backoff
6. Notifica resultado al usuario
```

### Otros Flujos
```
[A definir]
```

---

## ?? Seguridad en Integraciones

```
[ ] HTTPS obligatorio
[ ] Validación de certificados SSL
[ ] Timeouts configurados
[ ] Sanitización de inputs
[ ] Validación de respuestas
[ ] Logging sin exponer secrets
[ ] Rate limiting en cliente
[ ] Circuit breaker implementado
[ ] Encriptación de credenciales en reposo
```

---

## ?? Checklist de Integración

```
Por cada proveedor:
  [ ] Documentación leída
  [ ] Credenciales obtenidas
  [ ] Cliente HTTP creado
  [ ] DTOs modelados
  [ ] Manejo de errores implementado
  [ ] Logging configurado
  [ ] Reintentos configurados
  [ ] Tests unitarios creados
  [ ] Tests de integración (sandbox)
  [ ] Monitoreo configurado
  [ ] Documented en README
```

---

## ?? Referencias Cruzadas

- Ver: **4_SERVICIOS_Y_APIS.md** - Servicios locales
- Ver: **2_ARQUITECTURA.md** - Integración de sistemas
- Ver: **6_SEGUNDA_BASE_DATOS.md** - Si aplica sincronización de BD

---

## ?? Notas de Integraciones

> [Aquí irán decisiones técnicas sobre integraciones]

---

**Última actualización**: [Pendiente]
**Responsable**: [Usuario]
**Estado**: ?? Incompleto - Pendiente detalles de integraciones
