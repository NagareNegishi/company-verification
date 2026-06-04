# OpenRegistry Terms of Service — Compliance Checklist

**Source:** openregistry.sophymarine.com ToS — verified 2026-05-16
**For:** Fallback provider in company-verification library (open-source, non-commercial app)
**Note:** No formal API Access Agreement requiring a signature was found. This checklist covers what was verified in the ToS and what still needs confirmation.

---

## Permitted use (verified 2026-05-16)

- [x] Wrapping OpenRegistry as a configurable fallback is permitted
- [x] Restriction: do not redistribute responses as a commercial dataset-for-sale
- [x] Restriction: do not build a competing general-purpose registry proxy
- [x] Restriction: per-deployment credentials required — no bundling in library releases
- [x] Jurisdiction: governed by laws of England and Wales

---

## Technical and access details (verified 2026-06-05)

- [x] OAuth 2.1 with Dynamic Client Registration (RFC 7591) and PKCE — public client, no redirect server required
- [x] Anonymous access: 20 req/min per IP, no sign-up
- [x] Free signed-in tier: 30 req/min per user, 3 jurisdictions per rolling 60-second window
- [x] Paid tiers: Pro $9/month, Max $29/month
- [x] Closed-source (public repo is documentation only)
- [x] NZ Companies Office listed as covered jurisdiction, but upstream source is undocumented

---

## Not yet confirmed

- [ ] Formal developer or distribution agreement: not found in public docs — confirm before shipping the fallback adapter
- [ ] Attribution requirements: not mentioned in ToS — confirm none exist
- [ ] README notice for per-deployment credentials: confirm whether required
- [ ] Termination and data-deletion obligations: not found in ToS review — confirm

---

## If a formal agreement is found

Read it and fill in additional sections here following the structure of MBIE_API_Agreement_Checklist.md.
