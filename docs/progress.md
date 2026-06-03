# Project Progress

Companion to `company-verification-api.md`. Records what is done, what is next,
and decisions made along the way. Update this file as work progresses.

---

## Status: NZBN adapter in progress — waiting for API key approval

---

## Completed

### Bootstrap
- Dev container created and opened (`.devcontainer/`)
- Claude config in place (`.claude/settings.json`, `CLAUDE.md`)
- Firewall script in place (`.devcontainer/project-firewall.sh`)
- `LICENSE` (AGPL 3.0) committed
- `.gitignore` in place

### Core contract

- `CompanyCandidate` — sealed record with `RegistryId`, `Name`, `Country`, optional `AdditionalFields`
- `IVerificationProvider` — interface with `Search(name, country, cancellationToken)` and `SupportedCountries` for adapter self-declaration and generic routing
- `VerificationProviderBase` — abstract base class; owns `Search()` (validates, normalises, delegates to `SearchCore()`); adapters implement `SearchCore()`
- Input validation enforced in the contract layer: name (not null/whitespace, max 200 chars, no control characters, no angle brackets, Unicode allowed); country (ISO 3166-1 alpha-2, case-insensitive, normalised to uppercase)
- Boilerplate deleted: `Class1.cs`, `WeatherForecast.cs`, `WeatherForecastController.cs`, `UnitTest1.cs`
- Decision docs: `docs/decisions/company-candidate.md`, `docs/decisions/core-contract.md`
- `dotnet build` passes clean across all three projects

### .NET Solution scaffold
- Solution: `CompanyVerification.slnx` (`.NET 10` default format)
- Projects created and linked:
  - `CompanyVerification.Core` — classlib
  - `CompanyVerification.Api` — webapi with controllers
  - `CompanyVerification.Tests` — xunit
- `Api` and `Tests` reference `Core`
- `dotnet build` passes — all three projects compile clean

### NZBN adapter — in progress

- `api.business.govt.nz` added to `.devcontainer/project-firewall.sh` firewall allowlist
- NZBN API registration submitted at `portal.api.business.govt.nz` — sandbox key pending approval (up to one working day)
- Decision doc updated: `docs/decisions/nzbn-api.md` now includes verified registration steps and sandbox/production base URLs

---

## Next

1. **Smoke test** — once sandbox key arrives, run a live `curl` against the sandbox to confirm real response shape and entity type codes
2. **`NzbnResponse.cs`** — model response types from observed data; folder: `CompanyVerification.Core/Providers/Nz/`
3. **`NzbnProvider.cs`** — extend `VerificationProviderBase`; filter by status `"50"` and confirmed entity type codes; map to `CompanyCandidate`
4. **Test suite** — xUnit tests covering active returned, dissolved filtered, wrong type filtered, not-found, upstream outage
5. **`NzbnClient.cs`** — typed HTTP client; calls `api.business.govt.nz`
6. **Register in `Program.cs`** — DI wiring for `IHttpClientFactory`, typed client, `IVerificationProvider`
7. **Conformance YAML** — adapter declaration file (active statuses, full employer entity type list)
8. **API controller** — thin HTTP wrapper over `IVerificationProvider`

---

## Key decisions

| Decision | Choice | Reason |
|---|---|---|
| Solution format | `.slnx` | .NET 10 default; human-readable, no GUIDs |
| API style | Controllers | Explicit routing, easier to navigate while learning |
| State / monitoring | Out of scope | Service answers "active right now" only; change detection is a separate concern |
| Database | None | No store layer — service is stateless |
| NZ adapter | Native NZBN | OpenRegistry NZ implementation is undocumented — black box for primary market |
| NZBN adapter scope | All employer entity types | Purpose is legitimate employer verification, not company-only lookup — includes govt, societies, partnerships, trusts; excludes sole traders (no separate legal entity), unincorporated partnerships, and unknown legacy codes (B/I/D/F/N/S/T/Y/Z/G) |
| Fallback | OpenRegistry (candidate) | ToS verified; swappable if needed |
| MCP server | Not finalized | Defer until a real agent consumer exists |
| Country code format | ISO 3166-1 alpha-2 | Industry standard for APIs; unambiguous; case-insensitive accepted, normalised to uppercase |
| Contract enforcement | Interface + abstract base class | Interface for DI/testability; base class for guaranteed validation via Template Method pattern (`SearchCore`) |
| Adapter country declaration | `SupportedCountries` on interface | Enables generic routing layer; compiler enforces it on every adapter |
| Name search flexibility | Partial names accepted | Registries handle case-insensitive substring matching; typo tolerance is not provided |
| NZBN API access | Subscription key only | Read-only public data; OAuth2 not needed |
| NZBN development environment | Sandbox first | Sandbox key available immediately after approval; switch to production key once tested |
| NZBN adapter folder | `Core/Providers/Nz/` | Groups all country adapters under `Providers/`; isolates NZ-specific types and HTTP logic |
