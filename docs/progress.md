# Project Progress

Companion to `company-verification-api.md`. Records what is done, what is next,
and decisions made along the way. Update this file as work progresses.

---

## Status: NZBN adapter — compliance prep in progress; remaining items before coding starts

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

### NZBN adapter — compliance prep

- `CompanyVerification.Core/Providers/Nz/` folder created
- `NzbnFilter.cs` — entity status and entity type allowlists; source links to Procuret/nzbn-python
- `NzbnTermsTemplate.cs` — MBIE ToS clause verified line-by-line against the actual API Access Agreement PDF (November 2022); Schedule 1 excluded website categories used verbatim
- `Providers/Nz/.env.example` — placeholder for `NZBN_SUBSCRIPTION_KEY`; marked not finalised
- `conformance.yaml` — empty placeholder; to be filled before adapter is complete
- `.gitignore` updated: `docs/decisions/MBIE/*.pdf` added
- MBIE API Access Agreement PDF (`docs/decisions/MBIE/`) read and verified; gitignored


---

## Next

### Remaining compliance before coding starts

1. **A3 README notice** — add the library user credentials warning (clause 7.7): users must supply their own NZBN key; register at `portal.api.business.govt.nz`; sign the MBIE API Access Agreement
2. **Verify PDF not tracked** — `docs/decisions/MBIE/*.pdf` is gitignored but if the file was staged before the rule was added, run `git rm --cached` to untrack it

### Coding — NZBN adapter

3. **Smoke test** — once sandbox key arrives, run a live `curl` against the sandbox to confirm real response shape and entity type codes
4. **Fill `conformance.yaml`** — active statuses, full employer entity type list; matches `NzbnFilter.cs`
5. **`NzbnResponse.cs`** — model response types from observed data; must capture `sourceRegister` field from API response (needed for A2 attribution per clause 4.8); folder: `CompanyVerification.Core/Providers/Nz/`
6. **`NzbnClient.cs`** — typed HTTP client; calls `api.business.govt.nz`
7. **`NzbnProvider.cs`** — extend `VerificationProviderBase`; use `NzbnFilter` for status and entity type; map to `CompanyCandidate`; record `DateTimeOffset.UtcNow` as `searched_at` before the HTTP call; put `source_register` (from response) and `searched_at` (ISO 8601) into `AdditionalFields`
8. **Test suite** — xUnit tests covering active returned, dissolved filtered, wrong type filtered, not-found, upstream outage
9. **Register in `Program.cs`** — DI wiring for `IHttpClientFactory`, typed client, `IVerificationProvider`
10. **API controller** — thin HTTP wrapper over `IVerificationProvider`

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
| Adapter priority | API layer's concern, not the adapter's | Adapters declare which countries they handle; which adapter wins for a given country is decided at the composition root, not inside the adapter |
| Fallback availability | Optional — service deploys without it | Missing credentials: startup warning, fallback countries return "unsupported". Invalid credentials: startup alert, same runtime behaviour. Native adapters unaffected either way |
| Name search flexibility | Partial names accepted | Registries handle case-insensitive substring matching; typo tolerance is not provided |
| NZBN API access | Subscription key only | Read-only public data; OAuth2 not needed |
| NZBN development environment | Sandbox first | Sandbox key available immediately after approval; switch to production key once tested |
| NZBN adapter folder | `Core/Providers/Nz/` | Groups all country adapters under `Providers/`; isolates NZ-specific types and HTTP logic |
