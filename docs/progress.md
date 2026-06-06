# Project Progress

Companion to `company-verification-api.md`. Records what is done, what is next,
and decisions made along the way. Update this file as work progresses.

---

## Status: AU adapter — pre-coding setup in progress

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

### AU adapter — pre-coding setup

- ABR Web Services Agreement read and verified; compliance docs complete (`docs/decisions/ABR/`)
- `abr.business.gov.au` added to `.devcontainer/project-firewall.sh` firewall allowlist
- API endpoint confirmed: `https://abr.business.gov.au/abrxmlsearch/AbrXmlSearch.asmx/ABRSearchByNameAdvancedSimpleProtocol2017` (HTTP GET, returns XML)
- Issue draft created: `.github/drafts/issue-draft.md`


---

## Next

### Remaining items before NZBN coding starts

1. **A3 README notice** — add the library user credentials warning (clause 7.7): users must supply their own NZBN key; register at `portal.api.business.govt.nz`; sign the MBIE API Access Agreement
2. **Verify PDF not tracked** — `docs/decisions/MBIE/*.pdf` is gitignored but if the file was staged before the rule was added, run `git rm --cached` to untrack it

### Remaining items before AU (ABR) coding starts

3. **ABR entity type codes** — look up the full list of ABN entity type codes from the ABR API schema or WSDL; decide which count as valid employers (exclude individuals/sole traders); document in `docs/decisions/ABR/`
5. **README notice** — add the A3 library user notice from `ABR_Compliance_For_My_App.md`: users must register their own GUID at `abr.business.gov.au/Documentation/WebServiceRegistration`
6. **`Providers/Au/.env.example`** — `ABR_GUID` placeholder
7. **`AbrFilter.cs`** — active status set (`"Active"`); included entity type codes from step 4
8. **`conformance.yaml`** — AU statuses, AU entity types; no `source_register` (no attribution obligation)

### Coding — AU (ABR) adapter

9. **`AbrResponse.cs`** — XML response model; `ABRSearchByNameAdvancedSimpleProtocol2017` returns XML
10. **`AbrClient.cs`** — HTTP GET to `abr.business.gov.au`; GUID in query string; parse XML response; no sandbox
11. **`AbrProvider.cs`** — extend `VerificationProviderBase`; filter with `AbrFilter`; map to `CompanyCandidate`
12. **Test suite** — xUnit: active returned, cancelled filtered, wrong type filtered, not-found, upstream outage
13. **Register in `Program.cs`** — DI wiring alongside the NZBN adapter
14. **API controller** — thin HTTP wrapper over `IVerificationProvider` (shared with NZBN)

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
| Fallback availability | Optional — service deploys without it | Missing credentials: startup warning, fallback countries return "unsupported". Invalid credentials: startup alert, same runtime behaviour. Native adapters unaffected either way |
| Name search flexibility | Partial names accepted | Registries handle case-insensitive substring matching; typo tolerance is not provided |
| NZBN API access | Subscription key only | Read-only public data; OAuth2 not needed |
| NZBN development environment | Sandbox first | Sandbox key available immediately after approval; switch to production key once tested |
| NZBN adapter folder | `Core/Providers/Nz/` | Groups all country adapters under `Providers/`; isolates NZ-specific types and HTTP logic |
