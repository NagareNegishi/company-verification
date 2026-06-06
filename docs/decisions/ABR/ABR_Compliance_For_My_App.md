# ABR Compliance — My App

Australian Business Register (ABR) web services. Two uses: (a) the verification service shipped as a library others self-host, and (b) the same service embedded in my product where users register and use it free.

---

## Verified facts

Source: https://abr.business.gov.au (checked 2026-06-05)

- Registration is self-serve. GUID issued immediately. No human approval. (~2 minutes)
- Accepting the Web Services Agreement is required before the GUID is issued.
- Agreement URL: https://abr.business.gov.au/Tools/WebServicesAgreement — **read 2026-06-06**
- Name search is available on the free tier: `ABRSearchByName`, `ABRSearchByNameSimpleProtocol`
- API speaks SOAP or HTTP GET/POST with XML. JSON endpoint exists but covers limited methods only.
- Status values returned by the API: `Active`, `Cancelled`, `Not Active`
- No sandbox environment. The API is production-only and read-only.
- ABN is 11 digits. ACN (ASIC company number) is returned alongside the ABN for company entities.
- Trading names have not been updated since 2012. Legal name is the reliable match field.

---

## Part A — Drop-in text

### A1. Signup terms clause

No clause requires terms to be passed to end users or library consumers.
Third-party sharing is explicitly permitted.

Source: "You may, at your own risk, provide relevant extracts of the ABN Lookup
Web Services to third parties." (Use of ABN Lookup Web Services)

### A2. Attribution line

No attribution required. No source line, timestamp, or logo when displaying data.

One restriction: do not imply Commonwealth endorsement or affiliation with your
product.

Source: "You must not represent or imply, or allow any associated person to
represent or imply, that the Commonwealth endorses, approves, or is affiliated
in any way with you..." (Use of ABN Lookup Web Services)

### A3. Repo notice for library users

The licence is non-transferable. Each deployer must register their own GUID at
https://abr.business.gov.au/Documentation/WebServiceRegistration and accept the
Web Services Agreement. Do not ship a GUID in source code or config.

Source: "a non-exclusive non-transferable licence to access and use the ABN
Lookup Web Services" (Licence)

README notice to add:

> You must supply your own ABR GUID. Register at
> https://abr.business.gov.au/Documentation/WebServiceRegistration
> and accept the Web Services Agreement before use. Do not share or
> commit your GUID to source control.

---

## Part B — Watch-list

### B1. Library vs embedded obligations

The agreement addresses only "you" (the registered GUID holder) with no
distinction between library author and operator. The non-transferable licence
and the indemnity clause (which covers "unauthorised use") play out differently
depending on how the service is deployed:

| | Library (others self-host) | Embedded in my product |
|---|---|---|
| Who calls ABR | the deployer | only me, my GUID |
| Who registers + accepts agreement | each deployer | only me |
| Who carries indemnity | each deployer | me |
| Third-party data sharing | deployer may provide extracts "at own risk" | I provide results to users |

Library mode puts obligations on each deployer. Embedded mode puts them on me.
Keep the two separate in docs so a library user does not assume I have covered
their obligations.

Source: "a non-exclusive non-transferable licence" (Licence); "your use
(including unauthorised use)" (Indemnity)

### B2. Never ship a GUID

The licence is non-transferable. Another party using your GUID holds a right
they were never granted.

- No GUID in `.env`, `docker-compose.yml`, `devcontainer.json`, `appsettings.json`,
  CI secrets, fixtures, or commit history.
- Ship `.env.example` with a blank placeholder. Gitignore the real `.env`.

Source: "non-exclusive non-transferable licence" (Licence)

### B3. Redistribution and resale

Third-party sharing is explicitly permitted for relevant extracts. No resale or
redistribution prohibition. Charging for the service is not restricted.

Two limits on all third-party sharing:
- Relevant extracts only, not a full register mirror or bulk re-export.
- At your own risk: the indemnity applies to downstream sharing too.

Source: "You may, at your own risk, provide relevant extracts of the ABN Lookup
Web Services to third parties." (Use of ABN Lookup Web Services)

### B4. Excluded website categories

No excluded website categories (unlike MBIE, which has a Schedule 1 list).

The only related restriction: data must not be used in a way that is false,
misleading, or deceptive.

Source: "You must not... use the ABN Lookup Web Services in any way that is or
may be false, misleading or deceptive." (Use of ABN Lookup Web Services)

### B5. Termination and data deletion

No general data deletion obligation on termination. The agreement can be ended
by either party (30 days notice from ABR, immediate by you).

The only deletion obligation is record-specific: if ABR notifies you that a
particular entry has been withdrawn (e.g. for privacy), delete all copies
immediately. This applies during the agreement, not only at termination.

A stateless service with no database has nothing to delete for live API
responses. Cached or logged ABN records would be in scope.

Source: "If we notify you that specific information has been withdrawn you must
immediately take all reasonable action to delete all copies of that information
in your possession or control." (Use of ABN Lookup Web Services)

### B6. Indemnity

Broad, uncapped indemnity covering any loss, damage, cost, expense, claim,
proceeding, or liability that ABR incurs to any third party from your use,
including unauthorised use of your GUID.

- A leaked or shared GUID that causes a third-party claim comes back to you.
- ABR is also indemnified for its own lawful exercise of rights under the
  agreement (e.g. suspending your access).
- No monetary cap. No carve-out for ABR negligence.

Gate access, avoid logging raw ABN responses, and ensure every deployer holds
their own GUID (see B1, B2).

Source: "You indemnify us against any loss, damage, cost, expense, claim,
proceeding or liability of any kind that we (or our personnel) may incur to any
third party arising out of your use (including unauthorised use) of or access to
the ABN Lookup Web Services, or the lawful exercise of our rights pursuant to
this agreement." (Indemnity)

### B7. Ongoing obligations

Two obligations. Far fewer than MBIE.

1. **Keep contact details current.** Notify ABR of any change to email or
   postal address. ABR sends notices to the address on record, so a stale
   address means missed termination or withdrawn-record notices.

2. **Delete withdrawn records when notified.** If ABR tells you a specific
   record has been removed (e.g. for privacy), delete all copies immediately.

No audit, monitoring, incident-reporting, or usage-reporting obligations.

Source: "You are responsible for ensuring that your contact details provided to
us remain accurate and up to date." (Use of ABN Lookup Web Services)

---

## Pre-read checklist

- [x] Read full Web Services Agreement at https://abr.business.gov.au/Tools/WebServicesAgreement
- [x] Fill in Part A sections above
- [x] Fill in Part B sections above
- [x] Decide whether library mode and embedded mode carry different obligations (see B1)
