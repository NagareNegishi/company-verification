# Project Progress

Companion to `company-verification-api.md`. Records what is done, what is next,
and decisions made along the way. Update this file as work progresses.

---

## Status: NZBN adapter — tests and wiring remaining

---

## Completed

### NZBN adapter — setup and compliance

- `api.business.govt.nz` added to firewall allowlist; sandbox key obtained
- `NzbnFilter.cs` — active status codes and included entity type codes
- `NzbnTermsTemplate.cs` — MBIE ToS clause (clause 4.11); verified against API Access Agreement PDF
- `conformance.yaml` — empty placeholder; fill before adapter ships
- README — NZBN credentials notice added (clause 7.7)

### NZBN adapter — coding

- `NzbnResponse.cs` — `NzbnSearchResponse` + `NzbnEntity`; maps `nzbn`, `entityName`, `entityStatusCode`, `entityTypeCode` via `[JsonPropertyName]`
- `NzbnOptions.cs` — typed config; binds to `NZBN__SubscriptionKey`
- `NzbnProvider.cs` — single HTTP call via `GetFromJsonAsync`; filters via `NzbnFilter`; maps to `CompanyCandidate`

### Established patterns (follow for NZBN)

- Config: `IOptions<T>` per adapter; env vars use `__` separator (e.g. `NZBN__SubscriptionKey`)
- Testing: `FakeHttpHandler` intercepts HTTP; see `AbrProviderTests.cs` for test structure
- DI: `AddSingleton<IVerificationProvider, NzbnProvider>()` in `Program.cs`

---

## Next

### NZBN adapter — tests and wiring

1. Test suite — xUnit, same structure as `AbrProviderTests.cs`
2. Fill `conformance.yaml` — active statuses and entity type list
3. Register in `Program.cs` — DI wiring

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
| Name search flexibility | Partial names accepted | Registries handle case-insensitive substring matching; typo tolerance is not provided |
| NZBN API access | Subscription key only | Read-only public data; OAuth2 not needed |
| NZBN development environment | Sandbox first | Sandbox key available immediately after approval; switch to production key once tested |
| NZBN adapter folder | `Core/Providers/Nz/` | Groups all country adapters under `Providers/`; isolates NZ-specific types and HTTP logic |
