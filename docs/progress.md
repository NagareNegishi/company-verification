# Project Progress

Companion to `company-verification-api.md`. Records what is done, what is next,
and decisions made along the way. Update this file as work progresses.

---

## Status: CompanyVerification.Core 0.1.0-alpha live on NuGet.org. API deployment to Render configured; deploys automatically on merge to main.

---

## Completed

### Render deployment (branch: `feat/nuget-publish`)

- `Dockerfile` — multi-stage build; SDK image to compile, aspnet runtime image for the container; layer caching on restore
- `Program.cs` — `UseHttpsRedirection()` moved inside `IsDevelopment()`; Render terminates TLS at the proxy
- `.github/workflows/deploy.yml` — runs `dotnet test` on push to main; Render auto-deploys when check passes
- Render dashboard — `NZBN__SubscriptionKey` and `ABR__Guid` set; Auto-Deploy set to "After CI Checks Pass"

### NuGet publish preparation (branch: `feat/nuget-publish`)

- `CompanyVerificationOptions.cs` — combined options object; one property per adapter; scales as adapters are added
- `ServiceCollectionExtensions.cs` — `AddCompanyVerification(Action<CompanyVerificationOptions>)` in `Microsoft.Extensions.DependencyInjection` namespace
- `OptionsWarningService.cs` — `IHostedService` that logs a warning at startup for any missing credential; app continues running
- `AbrOptions`, `NzbnOptions` — dropped `required`, added `string.Empty` defaults; warning service covers the runtime check
- `Microsoft.Extensions.Hosting.Abstractions` added to Core (needed for `IHostedService` in a plain class library)
- `Program.cs` updated to use `AddCompanyVerification()` — manual wiring lines removed
- `CompanyVerification.Core.csproj` — NuGet package metadata added (all DO fields from best practices)
- `.github/workflows/publish.yml` — Trusted Publishing workflow; triggers on `v*` tags; uses `NuGet/login@v1` OIDC action
- nuget.org Trusted Publisher policy configured for `NagareNegishi/company-verification`
- `docs/adding-an-adapter.md` — checklist for new adapter authors
- `docs/nuget-publish-research.md` — package metadata values and DI decisions
- README Usage section updated with `AddCompanyVerification()` example
- `CompanyVerification.Core.csproj` — `PublishRepositoryUrl`, `IncludeSymbols`, `SymbolPackageFormat=snupkg` added; Source Link active via .NET 10 SDK (no package reference needed)
- `.github/workflows/publish.yml` — `Push symbols` step added; pushes `.snupkg` to nuget.org symbol server via V3 API

### NZBN adapter — setup and compliance

- `api.business.govt.nz` added to firewall allowlist; sandbox key obtained
- `NzbnFilter.cs` — active status codes and included entity type codes
- `NzbnTermsTemplate.cs` — MBIE ToS clause (clause 4.11); verified against API Access Agreement PDF
- `conformance.yaml` — complete; active status codes, included entity types, and additional fields declared
- README — NZBN credentials notice added (clause 7.7)

### NZBN adapter — coding

- `NzbnResponse.cs` — `NzbnSearchResponse` + `NzbnEntity`; maps `nzbn`, `entityName`, `entityStatusCode`, `entityTypeCode` via `[JsonPropertyName]`
- `NzbnOptions.cs` — typed config; binds to `NZBN__SubscriptionKey`
- `NzbnProvider.cs` — single HTTP call via `GetFromJsonAsync`; filters via `NzbnFilter`; maps to `CompanyCandidate`
- `NzbnProviderTests.cs` — test suite complete

### Established patterns (follow for NZBN)

- Config: `IOptions<T>` per adapter; env vars use `__` separator (e.g. `NZBN__SubscriptionKey`)
- Testing: `FakeHttpHandler` intercepts HTTP; see `AbrProviderTests.cs` for test structure
- DI: `AddSingleton<IVerificationProvider, NzbnProvider>()` in `Program.cs`

---

## Next

- Email `account@nuget.org` to reserve the `CompanyVerification` prefix (cosmetic — verified checkmark)
- `PackageIcon` — blocked on having a 128x128 PNG; wiring is ready to add once the file exists
- **NZBN `additionalFields` fix** (branch: `api/wire`) — partial work done; needs corrections before merge. Files to fix:
  - `NzbnResponse.cs` — revert `SourceRegister` addition; the NZBN API does not return a `sourceRegister` field (confirmed from live response and open-source library inspection)
  - `NzbnProvider.cs` — `searchedAt` capture and `AdditionalFields` dict are correct; change `["source_register"] = e.SourceRegister` to `["source_register"] = "NZBN"`
  - `NzbnProviderTests.cs` — remove `"sourceRegister"` from all fixtures; update assertion to `Assert.Equal("NZBN", results[0].AdditionalFields!["source_register"])`
  - `conformance.yaml` — change `source_register` entry: `source: adapter`; update description to "Always \"NZBN\"; the adapter serves the NZBN Register"
  - `docs/api-reference.md` — already updated; verify example JSON shows `"source_register": "NZBN"`
  - `docs/decisions/MBIE/MBIE_Compliance_For_My_App.md` — correct A2 note: `[Register name]` does not come from the API response; hardcode `"NZBN"` per adapter

---

## Key decisions

| Decision | Choice | Reason |
|---|---|---|
| Solution format | `.slnx` | .NET 10 default; human-readable, no GUIDs |
| API style | Controllers | Explicit routing, easier to navigate while learning |
| State / monitoring | Out of scope | Service answers "active right now" only; change detection is a separate concern |
| Database | None | No store layer — service is stateless |
| NZ adapter | Native NZBN | Direct, known scope; verified active status and entity types |
| NZBN adapter scope | All employer entity types | Purpose is legitimate employer verification, not company-only lookup — includes govt, societies, partnerships, trusts; excludes sole traders (no separate legal entity), unincorporated partnerships, and unknown legacy codes (B/I/D/F/N/S/T/Y/Z/G) |
| Fallback | None | OpenRegistry (initial candidate) ruled out June 2026 — MCP server, no REST API. No suitable alternative identified. Unsupported countries return "unsupported". |
| MCP server | Not finalized | Defer until a real agent consumer exists |
| Country code format | ISO 3166-1 alpha-2 | Industry standard for APIs; unambiguous; case-insensitive accepted, normalised to uppercase |
| Contract enforcement | Interface + abstract base class | Interface for DI/testability; base class for guaranteed validation via Template Method pattern (`SearchCore`) |
| Adapter country declaration | `SupportedCountries` on interface | Enables generic routing layer; compiler enforces it on every adapter |
| Adapter priority | API layer's concern, not the adapter's | Adapters declare which countries they handle; which adapter wins for a given country is decided at the composition root, not inside the adapter |
| Adapter config | `IOptions<T>` per adapter, root `.env` | Standard .NET pattern; env vars use `__` separator; root `.env.example` is the developer setup file; per-adapter files are reference only |
| Fallback availability | Optional — service deploys without it | Missing credentials: startup warning, fallback countries return "unsupported". Invalid credentials: startup alert, same runtime behaviour. Native adapters unaffected either way |
| `source_register` value | Hardcoded `"NZBN"` per adapter | NZBN API does not return a register name field. Schedule 2 obligation (clause 4.8) is on the Customer Site (frontend), not this API. Adapter name is the correct and stable value. |
| Name search flexibility | Partial names accepted | Registries handle case-insensitive substring matching; typo tolerance is not provided |
| NZBN API access | Subscription key only | Read-only public data; OAuth2 not needed |
| NZBN development environment | Sandbox first | Sandbox key available immediately after approval; switch to production key once tested |
| NZBN adapter folder | `Core/Providers/Nz/` | Groups all country adapters under `Providers/`; isolates NZ-specific types and HTTP logic |
| DI extension method shape | Single `Action<CompanyVerificationOptions>` | Per-adapter methods don't scale past 2-3 adapters; combined options object adds one property per new adapter with no call site changes |
| Missing credentials behaviour | Log warning at startup, continue running | Hard failure on startup rejected; operators need to know the app is misconfigured without a crash blocking other functionality |
| Options `required` keyword | Removed from options classes | `required` is compile-time only; options binding via reflection bypasses it; `OptionsWarningService` covers the runtime check |
