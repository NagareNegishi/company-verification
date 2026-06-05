# OpenRegistry Compliance — My App

OpenRegistry (openregistry.sophymarine.com) is the candidate fallback provider. It proxies national registries including the Australian Business Register. Two uses: (a) the verification service shipped as a library others self-host, and (b) the same service embedded in my product.

---

## Verified facts

Source: openregistry.sophymarine.com ToS — verified 2026-05-16
Source: openregistry.sophymarine.com + RFC 7591 — checked 2026-06-05

- Wrapping OpenRegistry as a configurable fallback is permitted under their ToS.
- Covers approximately 27 national registries. Australia (ABR) is included.
- New Zealand (NZ Companies Office) is listed as a covered jurisdiction but the upstream source they call is not documented. Do not treat as equivalent to the native NZBN adapter.
- Closed-source. The public repository is documentation only.
- Governed by the laws of England and Wales.
- Authentication uses OAuth 2.1 with Dynamic Client Registration (RFC 7591). Client registration is automated — no human approval. PKCE public client flow. No live redirect server required.
- Anonymous access: 20 requests per minute per IP. No sign-up required.
- Free signed-in tier: 30 requests per minute per user, 3 jurisdictions per rolling 60-second window.
- Paid tiers: Pro $9/month, Max $29/month, Enterprise by contact.

### Confirmed restrictions from ToS (2026-05-16)

1. Do not redistribute OpenRegistry responses as a commercial dataset-for-sale.
2. Do not use OpenRegistry to build a competing general-purpose registry proxy.
3. Per-deployment credentials are required by design. Shared credentials cannot be bundled in a library release. Each deployer registers their own OAuth client.

---

## Part A — Drop-in text

### A1. Signup terms clause

Not required. No clause in the ToS obligates the library author to pass terms to end users. No separate developer agreement exists (confirmed 2026-06-05).

### A2. Attribution line

Not required by OpenRegistry. The ToS contains no attribution clause for displayed data. Attribution for AU data shown to users is governed by the ABR Web Services Agreement, not by OpenRegistry.

### A3. Repo notice for library users

No mandatory notice text was found in the ToS. The per-deployment credential requirement is a technical constraint (each deployer self-registers via DCR), not a documented obligation to publish a warning. Include a practical note in the README as good practice.

---

## Part B — Watch-list

### B1. Formal developer or distribution agreement

Confirmed: no formal developer agreement exists (checked 2026-06-05 via site search and public docs). The ToS is the only governing document. No action required.

### B2. Rate limits at the free tier

The 3-jurisdiction cap per rolling 60-second window applies on the free tier. The current scope (NZ native, AU via fallback, everything else unsupported) stays within this limit. Note if scope expands.

### B3. Library vs embedded obligations

The ToS applies equally to both uses. No clause distinguishes between a library author and an end deployer. The three confirmed restrictions (no resale, no competing proxy, per-deployment credentials) apply to anyone running the adapter.

### B4. Termination and data deletion

Not applicable. OpenRegistry is a real-time proxy that holds no stored copy of registry data. Stopping use of the service leaves nothing to delete.

### B5. Jurisdiction mismatch

The agreement is governed by the laws of England and Wales. This is a minor mismatch for a NZ-primary project. Note it; no action required unless the project goes commercial.

---

## Pre-ship checklist

- [x] ToS verified — wrapping as configurable fallback permitted (2026-05-16)
- [x] Confirmed restrictions documented above
- [x] No separate developer or distribution agreement applies — ToS governs (2026-06-05)
- [x] No attribution requirements from OpenRegistry — ABR attribution tracked separately in ABR_Compliance_For_My_App.md (2026-06-05)
- [x] No mandatory README notice text — per-deployment credential note is good practice only (2026-06-05)
- [x] Termination/data deletion not applicable — stateless proxy, no stored data (2026-06-05)
