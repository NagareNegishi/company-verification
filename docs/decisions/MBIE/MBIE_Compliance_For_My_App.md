# MBIE Compliance — My App

Two uses: (a) the verification service shipped as a library others self-host, and (b) the same service embedded in my product where users register and use it free.

---

## Part A — Drop-in text

### A1. Signup terms clause (clause 4.11)

Paste into the ToS users accept at registration:

> **Use of New Zealand business register data**
> This service uses business information from registers administered by the New Zealand Ministry of Business, Innovation & Employment (MBIE) via the NZBN register. By using this service you agree that:
> 1. You will not use this data in breach of New Zealand law, including the Privacy Act 2020.
> 2. You will not use or display this data on, or with, any website or service that: incites hatred; promotes or facilitates violence, terrorism, or illegal activity; discriminates against or exploits any group; is misleading, pornographic, or defamatory; or infringes any person's privacy.
> 3. You will not resell, redistribute, or sub-license this data, or present it as your own register.
> 4. You will report any error or omission you notice in the data to us.
> 5. MBIE is the source and owner of this data, and may directly enforce these terms against you.
> 6. This data shows a business's legal registration status only. It does not confirm that a business is trading, hiring, or operating.

Items 2, 5, and 6 are non-negotiable.

### A2. Attribution line (clause 4.8 + Schedule 2)

Render wherever raw register fields are shown (legal name, NZBN, status, registration date):

> Data sourced from the [Register name] — searched [DD/MM/YYYY HH:MM NZ time]

- `[Register name]` = the `source_register` value from `additionalFields`. For NZ, this is always `"NZBN"` — the NZBN API does not return a register name field, so the adapter hardcodes it. Each adapter declares its own value; read it from the response rather than hardcoding it in the frontend.
- Timestamp = time of the actual search, not page load.
- Text only — no MBIE/NZBN logos.

### A3. Repo notice for library users (clause 7.7)

Put near install instructions in the README:

> **You need your own NZBN credentials.** This library calls the MBIE NZBN API and includes no keys. Register at portal.api.business.govt.nz, sign MBIE's API Access Agreement, and supply your own subscription key. Never commit your key. Do not use anyone else's key.

---

## Part B — Watch-list for my setup

### B1. Library vs embedded carry different obligations

| | Library (others self-host) | Embedded in my product |
|---|---|---|
| Who calls NZBN | the deployer | only me, my key |
| Who signs MBIE agreement | each deployer | only me |
| Who owes attribution + binds users | each deployer | me |
| Who carries indemnity (clause 13) | each deployer | me |

Library mode pushes obligations to the deployer. Embedded mode puts them all on me. Keep the two separate in docs so a library user doesn't assume I've covered what's actually theirs.

### B2. Never ship a key (clause 7.7)

- No subscription key in `.env`, `docker-compose.yml`, `devcontainer.json`, `project-firewall.sh`, CI secrets, fixtures, or commit history.
- Ship `.env.example` with a blank placeholder; gitignore the real `.env`.
- Conformance fixtures store response bodies only — never request headers with the key.

### B3. Signup gate is mandatory for the embedded product (clause 4.11)

- Free is fine; binding users is the requirement. The A1 clause must be in the terms accepted at signup, including MBIE's direct-enforcement right.
- No anonymous public tier that serves register data without accepting terms — "best endeavours to enforce" can't be shown with ungated access.

### B4. Attribution depends on what users see

- Raw register fields shown → A2 attribution required on screen.
- Only a derived result shown (verified ✓ / rating), raw fields never surfaced → on-screen attribution burden much lighter; A1 signup terms still apply.
- Decide and document which fields, if any, reach the end user. Derived-result-only is the lighter path.

### B5. No front door re-offers the API (clause 4.10(c))

- Returning my processed `CompanyCandidate` = providing content = allowed.
- A passthrough letting callers run arbitrary NZBN queries = re-offering the API = not allowed.
- MCP server is the riskiest: expose verification tools only (`verify(name, country) → result`), never a raw NZBN query tool. Design requirement if/when built.

### B6. My product must not be an Excluded Website (clause 4.4 + Schedule 1)

- Check the whole product surface. Any user-generated section needs moderation so the product can't fall into a Schedule 1 category (hate, violence, illegal, pornographic, defamatory, privacy-infringing).

### B7. Don't overstate verification (clause 14.1(a))

- Registered + active ≠ trading ≠ hiring. Keep the caveat visible in UI copy (A1 item 6) so a "verified" badge isn't misleading.

### B8. Bulk download = stored MBIE content (clause 18.2)

- Stateless / no database makes "destroy all MBIE content on termination" trivial — as long as I stay stateless.
- The monthly bulk download is a stored copy and reintroduces the deletion obligation. Prefer live API calls; treat bulk download as a deliberate, documented exception if used.

### B9. Keep MBIE API Specifications out of the public repo (clause 8)

- Endpoint shapes from the portal are fine to reference. MBIE-supplied specification documents and security details are confidential — don't commit them to the AGPL repo.

### B10. Commercial-licence wording (clause 4.7)

- The dual-license sells the software/service, not the NZBN data (free from the register). Word the fee as for code/hosting and keep it reasonable.

### B11. Ongoing duties after launch

- Respond to any MBIE compliance/audit request within 3 working days (clause 4.9) — monitor the contact email.
- Maintain a list of authorised users in my org (clause 7.5(b)).
- Notify MBIE immediately of unauthorised use or security violation; cease use if asked (clauses 7.1, 7.8).
- Report data faults to MBIE; require users to report to me (clause 6.1).
- Watch for agreement changes — 10 working days' notice; continued use = acceptance (clauses 2, 12).

### B12. Indemnity is personal (clause 13)

- Exposing the data in my product is at my own risk; I indemnify MBIE for downstream misuse and third-party claims arising from my use.
- The A1 binding reduces this but doesn't remove it. For a shared public endpoint, this is the main reason to gate access, prefer derived-result output, and get a legal read before public launch.

---

## Pre-launch checklist

- [ ] A1 clause in product signup terms (incl. MBIE direct-enforcement right)
- [ ] A2 attribution next to any raw register field (or confirmed: derived results only)
- [ ] A3 notice in library README
- [ ] No NZBN key in repo / fixtures / history; `.env.example` only
- [ ] MCP server (if built) exposes verification tools only
- [ ] Product surface reviewed against Schedule 1
- [ ] "Registered ≠ hiring" caveat visible in UI
- [ ] No bulk-download copy retained (or deletion path documented)
- [ ] No MBIE specification docs committed
- [ ] Account contact email monitored
