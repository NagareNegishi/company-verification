# Project Progress

Companion to `company-verification-api.md`. Records what is done, what is next,
and decisions made along the way. Update this file as work progresses.

---

## Status: Scaffolding complete — Core contract not yet started

---

## Completed

### Bootstrap
- Dev container created and opened (`.devcontainer/`)
- Claude config in place (`.claude/settings.json`, `CLAUDE.md`)
- Firewall script in place (`.devcontainer/project-firewall.sh`)
- `LICENSE` (AGPL 3.0) committed
- `.gitignore` in place

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

1. **Define Core contract** — `IVerificationProvider` interface + `CompanyCandidate` type in `Core`
2. **Delete boilerplate** — remove generated placeholder files (`Class1.cs`, `WeatherForecast.cs`, etc.)
3. **NZBN adapter** — first native adapter; add `api.business.govt.nz` to firewall script
4. **Conformance YAML** — adapter declaration file for NZBN
5. **Conformance test suite** — standard battery in `Tests`
6. **API controller** — thin HTTP wrapper over `IVerificationProvider`

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
