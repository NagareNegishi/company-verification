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

TODO — confirm whether OpenRegistry requires the library author to pass any terms to end users. No such clause was found in the ToS review, but a formal developer agreement may exist. See B1 below.

### A2. Attribution line

TODO — confirm whether OpenRegistry requires attribution text when showing data.

### A3. Repo notice for library users

Per-deployment credentials are required by design (confirmed restriction 3 above). Confirm whether this means a README notice is mandatory before writing it.

---

## Part B — Watch-list

### B1. Formal developer or distribution agreement

No formal developer agreement or library-distribution terms were found in public documentation during the ToS review. Confirm this is correct before the fallback adapter ships — check the site directly or contact Sophymarine.

### B2. Rate limits at the free tier

The 3-jurisdiction cap per rolling 60-second window applies on the free tier. The current scope (NZ native, AU via fallback, everything else unsupported) stays within this limit. Note if scope expands.

### B3. Library vs embedded obligations

TODO — confirm once B1 is resolved.

### B4. Termination and data deletion

TODO — confirm whether data must be deleted when access is terminated.

### B5. Jurisdiction mismatch

The agreement is governed by the laws of England and Wales. This is a minor mismatch for a NZ-primary project. Note it; no action required unless the project goes commercial.

---

## Pre-ship checklist

- [x] ToS verified — wrapping as configurable fallback permitted (2026-05-16)
- [x] Confirmed restrictions documented above
- [ ] Confirm no separate developer or distribution agreement applies (B1)
- [ ] Confirm attribution requirements (A2), or confirm none exist
- [ ] Decide whether a README notice is required for per-deployment credentials (A3)
- [ ] Confirm termination and data-deletion obligations (B4)
