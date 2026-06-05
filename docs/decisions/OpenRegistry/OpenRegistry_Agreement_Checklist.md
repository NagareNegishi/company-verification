# OpenRegistry Terms of Service — Compliance Checklist

**Source:** openregistry.sophymarine.com ToS — verified 2026-05-16
**For:** Fallback provider in company-verification library (open-source, non-commercial app)
**Note:** No formal API Access Agreement requiring a signature exists. The ToS is the governing document. All open items below were confirmed via web search on 2026-06-05.

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
- [x] Closed-source (public repo is documentation only, licensed CC-BY-4.0)
- [x] NZ Companies Office listed as covered jurisdiction, but upstream source is undocumented

---

## Previously unconfirmed — now resolved (2026-06-05)

- [x] Formal developer or distribution agreement: none exists — ToS is the only governing document
- [x] Attribution requirements: none required by OpenRegistry — ABR data attribution is governed by the ABR Web Services Agreement separately
- [x] README notice for per-deployment credentials: no mandatory text in ToS — good practice to document, not a stated obligation
- [x] Termination and data-deletion obligations: not applicable — OpenRegistry is a real-time proxy, no data is stored on their side or ours
