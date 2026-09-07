---
name: aspireify
description: >-
  **APPHOST WIRING SKILL** - Agentic AppHost authoring after `aspire init` drops the skeleton.
  Scans the repo, proposes a resource graph (PostgreSQL, Redis, RabbitMQ, etc.), edits the
  AppHost (C#, file-based C#, or TypeScript), wires Aspire.ServiceDefaults + OTel, validates with
  `aspire start`, then self-deactivates. Owns current AppHost authoring patterns: AddNextJsApp,
  AddViteApp, WithBrowserLogs(), generated .aspire/modules/, unified TS withEnvironment, endpoint
  references, and config/secret migration.
  USE FOR: AppHost wiring after aspire init, add resources to unwired AppHost, wire
  ServiceDefaults, wire OTel, AddNextJsApp, AddViteApp, WithBrowserLogs, add integrations to AppHost,
  propose resource graph, scan repo for services, C# AppHost editing, TypeScript AppHost editing,
  config/secret migration, endpoint references, aspireify.
  DO NOT USE FOR: start/stop/wait (use aspire-orchestration), deploy/publish/destroy (use
  aspire-deployment), logs/traces/metrics (use aspire-monitoring), first-run skeleton drop (use
  aspire-init), AppHost code on a repo with no aspire.config.json (use aspire-init first).
  INVOKES: aspire CLI (docs search, docs api search, add, integration list/search, start, stop, describe, ps).
  FOR SINGLE OPERATIONS: Route directly to aspire-orchestration for lifecycle, aspire-deployment
  for deploy, aspire-monitoring for telemetry.
license: MIT
metadata:
  author: Microsoft
  version: "0.0.1"
---

# aspireify — AppHost Wiring

> **After `aspire init` lands the skeleton, this skill does the actual wiring.** It scans the
> repository, proposes a resource graph, edits the AppHost, wires `Aspire.ServiceDefaults` + OTel,
> and validates via `aspire start`. Then it self-deactivates — the AppHost is now a first-class
> source file that the user or other skills edit directly.

## When to Activate

Activate when the AppHost skeleton exists but is **unwired or under-wired**:

| Signal | How to Detect | Confidence | Scope |
|--------|---------------|------------|-------|
| `aspire.config.json` present, AppHost stub has no resources | `aspire.config.json` exists + AppHost `Program.cs` has only `builder.CreateBuilder(args)` and `Build().Run()` | High | aspireify |
| C# AppHost with no `AddRedis`/`AddProject`/etc. | Grep AppHost for `Add` resource calls; none found | High | aspireify |
| File-based C# AppHost, unwired | `apphost.cs` with `#:sdk` and no resource declarations | High | aspireify |
| TypeScript AppHost, unwired | `apphost.ts` with no `add*` calls | High | aspireify |
| User asks to add resources/integrations to existing AppHost | Explicit "add Redis", "wire ServiceDefaults", "add Vite app" | n/a | aspireify |
| AppHost already fully wired | All services, dependencies, ServiceDefaults present | Low | Do **not** activate — route to aspire-orchestration |

## Prerequisites

| Requirement | Install |
|-------------|---------|
| .NET 10.0 SDK | https://dotnet.microsoft.com/download |
| Aspire CLI (curl installer) | `curl -sSL https://aspire.dev/install.sh \| bash` |
| Aspire CLI (NativeAOT global tool) | `dotnet tool install -g Aspire.Cli` |

## Default Workflow

### 1. Scan the Repository

Discover all service projects, Docker Compose files, and language-specific apps:

```bash
# .NET projects (look for .csproj, filter for non-test projects)
find . -name "*.csproj" -not -path "*/bin/*" -not -path "*/obj/*" | xargs grep -l "Microsoft.NET.Sdk"

# Node.js apps
find . -name "package.json" -not -path "*/node_modules/*" -not -path "*/.aspire/*"

# Docker Compose
find . -name "docker-compose*.y*ml" -not -path "*/.aspire/*"

# Python services
find . -name "requirements.txt" -o -name "pyproject.toml" -o -name "Pipfile" | grep -v node_modules
```

Build a picture of what needs to be modeled as Aspire resources.

### 2. Present Findings to the User

Summarize discovered services and propose a resource graph. For each service, propose:

- **Resource type**: `AddProject` (.NET), `AddViteApp` (Vite), `AddNextJsApp` (Next.js), `AddNodeApp` (Node), `AddContainer` (container), `AddExecutable` (native binary)
- **Dependencies**: What infrastructure each resource needs (Redis, Postgres, RabbitMQ, etc.)
- **References**: How resources reference each other via `WithReference` / `WithEnvironment`

### 3. Wire the AppHost

Edit the AppHost to declare resources. Follow the **3-tier API preference**:

| Tier | Source | Examples |
|------|--------|----------|
| **Tier 1** — First-party `Aspire.Hosting.*` | Built-in, no extra packages needed for core; integrations via `aspire add` | `AddRedis`, `AddPostgres`, `AddRabbitMq`, `AddProject`, `AddViteApp`, `AddNextJsApp` |
| **Tier 2** — Community Toolkit `CommunityToolkit.Aspire.Hosting.*` | `CommunityToolkit.Aspire.Hosting.*` NuGet | `AddGolangApp`, `AddMySql` |
| **Tier 3** — Raw fallbacks | No specific integration package | `AddContainer`, `AddDockerfile`, `AddExecutable` |

Before writing AppHost code, always check docs:

```bash
aspire docs search "<resource-type>"      # workflow guidance
aspire docs api search "<method>" --language csharp   # C# API reference
aspire docs api search "<method>" --language typescript  # TS API reference
```

### 4. Configure Dependencies

- **ServiceDefaults**: Ensure every .NET service project references `Aspire.ServiceDefaults` and calls `builder.AddServiceDefaults()` in `Program.cs`.
- **OTel**: ServiceDefaults wires OTel automatically. For non-.NET services, use `AddOpenTelemetry` via container environment or OTel collector.
- **Connection strings**: Use `WithReference(resource)` and `WithEnvironment(name, expression)` — never hardcode URLs.

### 5. Validate

```bash
aspire start
aspire wait <resource>   # wait for each resource
aspire describe          # inspect state and endpoints
```

Iterate until all resources are healthy, then `aspire stop` and hand off to `aspire-orchestration` for ongoing lifecycle management.

## AppHost Editing Patterns

### C# AppHost

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Infrastructure resources — declare first
var redis = builder.AddRedis("redis");
var postgres = builder.AddPostgres("postgres");
var api = builder.AddProject<Projects.MyApi>("api")
    .WithHttpHealthCheck("/health");

// Application resources — reference infrastructure
var web = builder.AddViteApp("web", "../web")
    .WithReference(redis)
    .WithReference(postgres)
    .WithReference(api)
    .WaitFor(redis)
    .WaitFor(postgres)
    .WaitFor(api)
    .WithHttpHealthCheck("/health")
    .WithBrowserLogs();

builder.Build().Run();
```

See [references/apphost-wiring.md](references/apphost-wiring.md) for the full API reference, including TypeScript patterns, ServiceDefaults wiring, and config/secret migration.

## Key Rules

- **Never install the obsolete Aspire workload** (`dotnet workload install aspire`)
- **Always use `aspire docs search` / `aspire docs api search` before writing AppHost code** — don't guess APIs
- **Never hardcode URLs** — use endpoint references (`WithReference`, `WithEnvironment` with expressions)
- **Never overwrite existing files** — augment and merge
- **Never edit `.aspire/modules/` directly** in TypeScript AppHosts — use `aspire add <package>` to regenerate
- **Always `--non-interactive`** for agent execution
- **Always `aspire start`** — never `dotnet run` on AppHosts (see aspire-orchestration safety guardrails)
- **Always `aspire wait <resource>`** before interacting with any resource
- **Always stop** (`aspire stop`) when wiring iterations are complete, before handing off

## Self-Deactivation

Once the AppHost is fully wired and validated, this skill **self-deactivates**. The AppHost is now a source file that you, the user, or other skills edit directly. For ongoing lifecycle (start/stop/wait), route to [`aspire-orchestration`](../aspire-orchestration/SKILL.md). For deployment, route to [`aspire-deployment`](../aspire-deployment/SKILL.md). For telemetry, route to [`aspire-monitoring`](../aspire-monitoring/SKILL.md).

## Project-Local Skill Override

If `.agents/skills/aspireify/SKILL.md` exists (installed by `aspire init` in current Aspire), **warn the user** and **defer to the project-local copy**. Repo-specific guidance should not be overridden by this in-plugin sibling.

Project-local `aspireify` content version is aligned to the consumer's Aspire CLI version, so it is authoritative for AppHost API patterns.

## Handoff Rules

| Scenario | Route To |
|----------|----------|
| Start/stop/wait/restart AppHost lifecycle | → `aspire-orchestration` skill |
| Deploy, publish, destroy, pipeline steps | → `aspire-deployment` skill |
| Logs, traces, metrics, dashboard, browser logs | → `aspire-monitoring` skill |
| AppHost wiring (add resources, integrations, ServiceDefaults, OTel) | → `aspireify` skill (this skill) |
| First-run skeleton drop (`aspire init` / `aspire new`) | → `aspire-init` skill |

## References

- [references/apphost-wiring.md](references/apphost-wiring.md) — C# and TypeScript AppHost API lookup, resource graph patterns, ServiceDefaults/OTel wiring, endpoint references, and config/secret migration
- [../aspire-orchestration/references/app-commands.md](../aspire-orchestration/references/app-commands.md) — After `aspire init` — hand off to `aspireify` (scan → propose → wire → validate)
- [../aspire-orchestration/references/safety-guardrails.md](../aspire-orchestration/references/safety-guardrails.md) — `aspire start` / `aspire wait` / `aspire stop` safety rules
- [../aspire-orchestration/references/detection.md](../aspire-orchestration/references/detection.md) — C# vs TypeScript AppHost detection
- [../aspire-deployment/references/javascript.md](../aspire-deployment/references/javascript.md) — `AddViteApp`, `AddNextJsApp`, `AddNodeApp` patterns and production serving model
- [../aspire-deployment/references/preflight.md](../aspire-deployment/references/preflight.md) — Parameter and secret preflight (for AppHost parameters this skill authors)
- [../aspire-monitoring/references/monitoring.md](../aspire-monitoring/references/monitoring.md) — Browser telemetry via `WithBrowserLogs()` discovery
- [../aspire/references/aspire-13-3-breaking-changes.md](../aspire/references/aspire-13-3-breaking-changes.md) — Scrub 13.3 breaking changes from AppHost code
