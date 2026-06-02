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

- No database — service is stateless.

- Adapter filtering: return only `ACTIVE` entities, company types only.
  What counts as active/company is the adapter's decision, declared in
  its YAML conformance file.