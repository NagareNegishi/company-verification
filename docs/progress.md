# Project Progress

Companion to `company-verification-api.md`. Records what is done, what is next,
and decisions made along the way. Update this file as work progresses.

---

## Status: Core contract complete — NZBN adapter not yet started

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
- `IVerificationProvider` — interface with `Search(name, country, cancellationToken)`
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

---

## Next

1. **NZBN adapter** — first native adapter; add `api.business.govt.nz` to firewall script; extend `VerificationProviderBase`, implement `SearchCore()`
2. **Conformance YAML** — adapter declaration file for NZBN (active statuses, company entity types)
3. **Conformance test suite** — standard battery in `Tests` covering active, dissolved, non-company, not-found, and upstream outage cases
4. **API controller** — thin HTTP wrapper over `IVerificationProvider`

---

## Key decisions

| Decision | Choice | Reason |
|---|---|---|
| Solution format | `.slnx` | .NET 10 default; human-readable, no GUIDs |
| API style | Controllers | Explicit routing, easier to navigate while learning |
| State / monitoring | Out of scope | Service answers "active right now" only; change detection is a separate concern |
| Database | None | No store layer — service is stateless |
| NZ adapter | Native NZBN | OpenRegistry NZ implementation is undocumented — black box for primary market |
| Fallback | OpenRegistry (candidate) | ToS verified; swappable if needed |
| MCP server | Not finalized | Defer until a real agent consumer exists |
| Country code format | ISO 3166-1 alpha-2 | Industry standard for APIs; unambiguous; case-insensitive accepted, normalised to uppercase |
| Contract enforcement | Interface + abstract base class | Interface for DI/testability; base class for guaranteed validation via Template Method pattern (`SearchCore`) |
| Name search flexibility | Partial names accepted | Registries handle case-insensitive substring matching; typo tolerance is not provided |
