# Project Progress

Companion to `company-verification-api.md`. Records what is done, what is next,
and decisions made along the way. Update this file as work progresses.

---

## Status: CompanyVerification.Core 0.1.0-alpha live on NuGet.org. API deployment to Render configured; deploys automatically on merge to main.

---

### Established patterns (follow for new adapters)

- Config: `IOptions<T>` per adapter; env vars use `__` separator (e.g. `NZBN__SubscriptionKey`)
- Testing: `FakeHttpHandler` intercepts HTTP; see `AbrProviderTests.cs` for test structure
- DI: `AddSingleton<IVerificationProvider, NzbnProvider>()` in `Program.cs`

---

## Next

- Email `account@nuget.org` to reserve the `CompanyVerification` prefix (cosmetic — verified checkmark)
- `PackageIcon` — blocked on having a 128x128 PNG; wiring is ready to add once the file exists

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
