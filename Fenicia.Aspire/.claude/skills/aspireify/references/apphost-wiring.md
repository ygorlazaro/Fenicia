# AppHost Wiring Reference

> **Purpose**: C# and TypeScript AppHost API lookup, resource graph patterns, ServiceDefaults/OTel
> wiring, endpoint references, and config/secret migration. This is the authoritative reference
> for `aspireify` — the AppHost authoring skill.

## API Tier Preference

Always prefer Tier 1 first-party `Aspire.Hosting.*` APIs. Fall back to Tier 2 community toolkit,
then Tier 3 raw fallbacks.

| Tier | Source | When to use | Examples |
|------|--------|-------------|----------|
| **Tier 1** | `Aspire.Hosting.*` (first-party) | Default for all resources | `AddRedis`, `AddPostgres`, `AddRabbitMq`, `AddProject`, `AddViteApp`, `AddNextJsApp` |
| **Tier 2** | `CommunityToolkit.Aspire.Hosting.*` | No first-party resource exists | `AddGolangApp`, `AddMySql`, `AddElasticSearch` |
| **Tier 3** | Raw fallbacks (no integration package) | Nothing else covers the resource | `AddContainer`, `AddDockerfile`, `AddExecutable` |

## Resource Discovery

Scan the repo to discover what needs to be modeled:

### .NET Projects (services)

```bash
find . -name "*.csproj" -not -path "*/bin/*" -not -path "*/obj/*" \
  | xargs grep -l "Microsoft.NET.Sdk.Web\|Microsoft.NET.Sdk" \
  | grep -v AppHost | grep -v ServiceDefaults
```

Each `.csproj` referencing `Microsoft.NET.Sdk.Web` or a shared framework is a service resource.
Use `AddProject<Projects.YourProject>("name")` in C# or `addProject("name", "./path")` in TS.

### Node.js Apps

```bash
find . -name "package.json" -not -path "*/node_modules/*" -not -path "*/.aspire/*"
```

- **Vite**: Look for `vite` in `devDependencies` → `AddViteApp` / `addViteApp`
- **Next.js**: Look for `next` in `dependencies` → `AddNextJsApp` / `addNextJsApp`
- **Generic Node**: Any `package.json` with a start script → `AddNodeApp` / `addNodeApp`

### Docker Compose

```bash
find . -name "docker-compose*.y*ml" -not -path "*/.aspire/*"
```

For each compose service, consider whether to migrate to a first-party resource type or keep as
a container. Prefer `AddContainer` only when no native equivalent exists.

### Python / Go / Other

- **Python**: `requirements.txt` / `pyproject.toml` → `AddContainer` or `AddExecutable`
- **Go**: Use `CommunityToolkit.Aspire.Hosting.Golang` (`AddGolangApp`) in Tier 2, or
  `AddContainer` / `AddExecutable` in Tier 3

## Resource Graph Patterns

### Infrastructure Resources

Declare infrastructure first — application resources depend on them:

```csharp
var redis = builder.AddRedis("redis");
var postgres = builder.AddPostgres("postgres");
var rabbit = builder.AddRabbitMq("rabbit");
```

```typescript
const redis = await builder.addRedis("redis");
const postgres = await builder.addPostgres("postgres");
const rabbit = await builder.addRabbitMq("rabbit");
```

### Application Resources

```csharp
var api = builder.AddProject<Projects.MyApi>("api")
    .WithHttpHealthCheck("/health");
```

```typescript
const api = await builder
  .addProject("api", "../api", {
    // project reference
  })
  .withHttpHealthCheck("/health");
```

### Resource References

Wire dependencies between resources using `WithReference` (C#) / `withReference` (TS) and
`WaitFor` (C#) / `waitFor` (TS):

```csharp
// C#
var web = builder.AddViteApp("web", "../web")
    .WithReference(redis)        // injects connection string / endpoint
    .WithReference(api)
    .WaitFor(redis)              // ensures redis is running first
    .WaitFor(api)
    .WithHttpHealthCheck("/health")
    .WithBrowserLogs();
```

```typescript
// TypeScript
const web = await builder
  .addViteApp("web", "../web")
  .withReference(redis)
  .withReference(api)
  .waitFor(redis)
  .waitFor(api)
  .withHttpHealthCheck("/health")
  .withBrowserLogs();
```

## C# AppHost API Reference

### Infrastructure Resources

| Resource | Method | Package | Example |
|----------|--------|---------|---------|
| Redis | `AddRedis` | `Aspire.Hosting.Redis` | `builder.AddRedis("redis")` |
| PostgreSQL | `AddPostgres` | `Aspire.Hosting.Postgres` | `builder.AddPostgres("postgres")` |
| RabbitMQ | `AddRabbitMq` | `Aspire.Hosting.RabbitMQ` | `builder.AddRabbitMq("rabbit")` |
| MongoDB | `AddMongoDB` | `Aspire.Hosting.MongoDB` | `builder.AddMongoDB("mongo")` |
| MySQL | `AddMySql` | `Aspire.Hosting.MySql` | `builder.AddMySql("mysql")` |
| SQL Server | `AddSqlServer` | `Aspire.Hosting.SqlServer` | `builder.AddSqlServer("sql")` |
| Orleans | `AddOrleans` | `Aspire.Hosting.Orleans` | `builder.AddOrleans("orleans")` |
| ClickHouse | `AddClickHouse` | `Aspire.Hosting.ClickHouse` | `builder.AddClickHouse("clickhouse")` |
| Elasticsearch | `AddElasticsearch` | `Aspire.Hosting.Elasticsearch` | `builder.AddElasticsearch("elastic")` |
| KeyVault | `AddAspireKeyVault` | `Aspire.Hosting.KeyVault` | `builder.AddAspireKeyVault("kv")` |
| Azure App Configuration | `AddAppConfiguration` | `Aspire.Hosting.AppConfig` | `builder.AddAppConfiguration("config")` |

### Project Resources

| Method | Purpose | Example |
|--------|---------|---------|
| `AddProject<T>` | Strongly-typed .NET project reference | `builder.AddProject<Projects.Api>("api")` |
| `AddProject` (string overload) | Project reference by path | `builder.AddProject("api", "../src/Api/Api.csproj")` |
| `WithHttpHealthCheck` | Add HTTP health check endpoint | `.WithHttpHealthCheck("/health")` |
| `WithReference` | Inject connection string / endpoint refs | `.WithReference(redis)` |
| `WaitFor` | Ensure dependency runs first | `.WaitFor(redis)` |
| `WithBrowserLogs` | Capture browser console/network/screenshots | `.WithBrowserLogs()` |

### JavaScript / Frontend Resources

| Method | Package | Use when |
|--------|---------|----------|
| `AddViteApp` | `Aspire.Hosting.JavaScript` | Vite-based framework dev server |
| `AddNextJsApp` | `Aspire.Hosting.JavaScript` | Next.js app |
| `AddNodeApp` | `Aspire.Hosting.JavaScript` | Generic Node process |
| `AddJavaScriptApp` | `Aspire.Hosting.JavaScript` | Package-script-driven app |

**Package-manager helpers** (C#): `WithNpm(...)`, `WithYarn(...)`, `WithPnpm(...)`, `WithBun(...)`
**Package-manager helpers** (TS): `withNpm(...)`, `withYarn(...)`, `withPnpm(...)`, `withBun(...)`

**Note**: `AddNextJsApp` and JavaScript publish methods are experimental. In C# AppHosts, suppress
warnings with `#pragma warning disable ASPIREJAVASCRIPT001` rather than blanket suppression.

### Raw Fallbacks (Tier 3)

| Method | Use when |
|--------|----------|
| `AddContainer` | Pre-built container image |
| `AddDockerfile` | Source with a Dockerfile |
| `AddExecutable` | Native binary on disk |

### Endpoint Configuration

| Method | Purpose |
|--------|---------|
| `WithHttpEndpoint` | Explicit HTTP endpoint (port, URL, env var) |
| `WithHttpHealthCheck` | Mark an endpoint as a health check |
| `WithExternalHttpEndpoints` | Expose resource endpoints externally (dashboard, browser) |
| `ExcludeReferenceEndpoint` | Prevent endpoint from being injected into referencing resources |

### Lifecycle Hooks

| Method | Phase |
|--------|-------|
| `SubscribeBeforeStart` | Before a resource starts |
| `SubscribeAfterResourcesCreated` | After all resources are created but before any start |

### Custom Resource Commands

| Method | Purpose |
|--------|---------|
| `WithCommand` | Add a custom resource command |
| `WithHttpCommand` | HTTP-based resource command |

See [aspire docs](https://aspire.dev) for current API signatures — always verify with:
```bash
aspire docs api search "<method-name>" --language csharp
```

## TypeScript AppHost API Reference

TypeScript AppHost uses `apphost.ts` with the `@aspire/apphost` package. The `.aspire/modules/`
directory contains generated TypeScript modules — **never edit directly**. Use `aspire add` to
regenerate, `aspire restore` to recover.

### Unified `withEnvironment`

The unified `withEnvironment(name, value)` replaces all deprecated per-kind helpers:

```typescript
// ✅ Current (Aspire 13.3+)
.withEnvironment("MY_VAR", "literal-value")
.withEnvironment("CONNECTION", redis.connectionString)
.withEnvironment("PORT", api.endpoint.port)

// ❌ Deprecated (remove)
.withEnvironmentConnectionString("MY_VAR", resource)
.withEnvironmentEndpoint("MY_VAR", endpoint)
.withEnvironmentParameter("MY_VAR", param)
.withEnvironmentExpression("MY_VAR", expr)
.withEnvironmentFromOutput("MY_VAR", output)
.withEnvironmentFromKeyVaultSecret("MY_VAR", secret)
```

The unified `withEnvironment(name, value)` accepts any of: a plain `string`,
`ReferenceExpression`, `EndpointReference`, parameter builder, connection string resource
builder, or `IExpressionValue`.

### Frontend Resources (TypeScript)

| Method | Use when |
|--------|----------|
| `addViteApp` | Vite-based framework dev server |
| `addNextJsApp` | Next.js app |
| `addNodeApp` | Generic Node process |
| `addJavaScriptApp` | Package-script-driven app |

```typescript
const frontend = await builder.addViteApp("frontend", "./frontend")
  .withReference(redis)
  .waitFor(redis)
  .withHttpHealthCheck("/health")
  .withBrowserLogs();
```

## ServiceDefaults Wiring

Every .NET service project must:

1. Reference `Fenicia.Aspire.ServiceDefaults.csproj` (or the shared defaults project)
2. Call `builder.AddServiceDefaults()` in `Program.cs`

```xml
<!-- In service .csproj -->
<Project Sdk="Microsoft.NET.Sdk.Web">
  <ItemGroup>
    <ProjectReference Include="..\Fenicia.Aspire.ServiceDefaults\Fenicia.Aspire.ServiceDefaults.csproj" />
  </ItemGroup>
</Project>
```

```csharp
// In service Program.cs
var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();  // wires OTel, health checks, service discovery, resilience
```

`AddServiceDefaults()` from the `Extensions.cs` in ServiceDefaults wires:

- **OpenTelemetry** — logs, metrics (AspNetCore, HttpClient, Runtime), tracing (AspNetCore, HttpClient)
- **Health checks** — `/health` and `/alive` endpoints
- **Service discovery** — `AddServiceDiscovery()` for typed HTTP clients
- **Resilience** — `AddStandardResilienceHandler()` on HTTP clients

For non-.NET services, OTel is wired via the container's environment (OTLP endpoint is auto-injected
by Aspire). No additional code is needed.

## Config / Secret Migration

### Parameters

AppHost parameters are declared with `builder.Configuration["Parameters:..."]` or the
`AddParameter` / `DefineParameter` API. When migrating from config files:

```csharp
// C# — parameter with secret
var redisPassword = builder.AddPasswordParameter("redis-password");

// C# — non-secret parameter
var postgresPassword = builder.AddParameter("postgres-password");
```

```bash
# Set secrets locally
aspire secret set "Parameters:redis-password" "MySecretValue"

# Deploy with environment variables (non-interactive)
Parameters__redis_password="MySecretValue" aspire deploy --non-interactive
```

### Connection Strings

When adding a resource that needs a connection string, use `WithReference(resource)` — Aspire
injects the connection string environment variable automatically. The exact env var name is
target-specific and documented in the integration package.

### AppSettings Migration

Move infrastructure connection strings from `appsettings.json` into the AppHost resource model.
Service projects should **not** contain hard-coded connection strings. Instead:

1. Declare the resource in the AppHost (e.g., `builder.AddRedis("redis")`)
2. Reference it from the service: `.WithReference(redis)`
3. Remove the connection string from `appsettings.json` — it now flows from AppHost

```csharp
// AppHost.cs
var redis = builder.AddRedis("redis");

var api = builder.AddProject<Projects.Api>("api")
    .WithReference(redis);  // injects "Redis__ConnectionString" or similar
```

## Validation Patterns

### Basic Validation

```bash
# Start the AppHost (background, agent-safe)
aspire start

# Wait for each resource
aspire wait redis
aspire wait apiservice
aspire wait webfrontend

# Inspect state
aspire ps
aspire describe --format Json
```

### Machine-Readable Inspection

```bash
# Resource list
aspire ps --format Json | jq '.[] | {name, displayName, state}'

# Specific resource details
aspire describe apiservice --format Json | jq '.endpoints'

# Include hidden resources (proxies, helpers, migrations)
aspire ps --include-hidden --format Json
```

### File-Lock Recovery

If the AppHost is already running and you edit code:

```bash
# ✅ Stop first, then rebuild/restart
aspire stop
# make code changes...
aspire start
```

Never use `dotnet build` while Aspire holds locks on `bin/` / `obj/`.

### Browser Logs Validation

When `WithBrowserLogs()` is wired, browser console logs, network requests, and screenshots
appear alongside server telemetry. Verify via the dashboard and:

```bash
aspire otel logs webfrontend
```

## Breaking Change Scrub (13.3+)

Before recommending or generating AppHost code, scrub for these patterns:

| Old (13.2) | New (13.3+) |
|------------|-------------|
| `--log-level` on `aspire publish`/`deploy` | `--pipeline-log-level` |
| Dashboard MCP (`ASPIRE_DASHBOARD_MCP_ENDPOINT_URL`) | `aspire agent init` (AppHost-level MCP) |
| `NameOutput` (Azure Network resources) | `NameOutputReference` |
| `OtlpEndpointEnvironmentVariableName` property | Removed — OTLP managed automatically |
| `AksSkuTier` enum | Removed — AKS defaults to Free SKU |
| `AddAndPublishPromptAgent` API | `AddPromptAgent` |
| `dotnet new aspire-py-starter` | `aspire new aspire-py-starter` (CLI template) |
| TS `withEnvironmentExpression(...)` etc. | `withEnvironment(name, value)` (unified) |
| `ASPIREEXTENSION001` | `ASPIREJAVASCRIPT001` |
| `aspire init` wires resources | `aspire init` drops skeleton; `aspireify` does the wiring |

## Docs Lookup

Always verify API before writing AppHost code:

```bash
aspire docs search "<resource-type or pattern>"
aspire docs get "<slug>"
aspire docs api search "<method-name>" --language csharp
aspire docs api search "<method-name>" --language typescript
aspire docs api get "<id-from-search>"
```

If `aspire docs` is unavailable, use the official [aspire.dev](https://aspire.dev) docs — never
outdated blog posts or workload-era documentation.
