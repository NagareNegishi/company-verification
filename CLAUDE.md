## Project Overview

Answers one question: given a company name and country, is this company legally active? 
Pluggable adapter per country registry — new country = new adapter, never touch core.

**Solution**:
- `CompanyVerification.Core/` — contract + adapters
- `CompanyVerification.Api/` — HTTP
- `CompanyVerification.Tests/` — xUnit

---

## Rules

- Every new adapter must add its registry hostname to
  `.devcontainer/project-firewall.sh` before any code can reach it.

- Adapter filtering: return only `ACTIVE` entities, company types only.
  What counts as active/company is the adapter's decision, declared in
  its YAML conformance file.

- Network is restricted by `.devcontainer/project-firewall.sh`. Before fetching any URL, check that its host is whitelisted there. If it isn't, use web search instead of fetch.

- Never edit, bypass, or override the settings files (`.claude/settings.json`, `.claude/settings.local.json`) or their permission rules.