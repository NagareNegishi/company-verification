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

The agreement contains no clause requiring terms to be passed to end users or
library consumers. Third-party data sharing is explicitly permitted with no
downstream terms obligation attached.

Source: "You may, at your own risk, provide relevant extracts of the ABN Lookup
Web Services to third parties." (Use of ABN Lookup Web Services)

### A2. Attribution line

The agreement imposes no attribution requirement. There is no clause requiring
a source line, timestamp, or logo when displaying data.

The one related obligation is negative: you must not imply that the Commonwealth
endorses, approves, or is affiliated with your product or use of the data.

Source: "You must not represent or imply, or allow any associated person to
represent or imply, that the Commonwealth endorses, approves, or is affiliated
in any way with you..." (Use of ABN Lookup Web Services)

### A3. Repo notice for library users

The licence granted by the agreement is non-transferable. Each deployer must
register their own GUID at https://abr.business.gov.au/Documentation/WebServiceRegistration
and accept the Web Services Agreement. Shipping a GUID in source code or
configuration would allow others to use a licence that is not theirs.

Source: "a non-exclusive non-transferable licence to access and use the ABN
Lookup Web Services" (Licence)

README notice to add:
  You must supply your own ABR GUID. Register at
  https://abr.business.gov.au/Documentation/WebServiceRegistration
  and accept the Web Services Agreement before use. Do not share or
  commit your GUID to source control.

---

## Part B — Watch-list

### B1. Library vs embedded obligations

The agreement makes no formal distinction between a library author and an
operator — it addresses only "you" (the registered GUID holder). However, the
non-transferable licence and the indemnity clause (which explicitly covers
"unauthorised use") have clear practical consequences for each mode:

| | Library (others self-host) | Embedded in my product |
|---|---|---|
| Who calls ABR | the deployer | only me, my GUID |
| Who registers + accepts agreement | each deployer | only me |
| Who carries indemnity | each deployer | me |
| Third-party data sharing | deployer may provide extracts "at own risk" | I provide results to users |

Library mode pushes all obligations to the deployer. Embedded mode puts them
all on me. Keep the two separate in docs so a library user does not assume I
have covered what is actually theirs.

Source: "a non-exclusive non-transferable licence" (Licence); "your use
(including unauthorised use)" (Indemnity)

### B2. Never ship a GUID

The licence is non-transferable. Another party using your GUID is exercising a
right they were never granted. The agreement does not name GUID sharing
explicitly, but the non-transferable licence makes the practical obligation clear.

- No GUID in `.env`, `docker-compose.yml`, `devcontainer.json`, `appsettings.json`,
  CI secrets, fixtures, or commit history.
- Ship `.env.example` with a blank placeholder; gitignore the real `.env`.

Source: "non-exclusive non-transferable licence" (Licence)

### B3. Redistribution and resale

TODO — confirm whether data returned by the API can be passed to third parties, and on what terms.

### B4. Excluded website categories

TODO — confirm whether the agreement prohibits displaying data on certain types of website.

### B5. Termination and data deletion

TODO — confirm whether data must be deleted when the agreement is terminated.

### B6. Indemnity

TODO — note any indemnity clause affecting the library author.

### B7. Ongoing obligations

TODO — note any audit, monitoring, or notification obligations.

---

## Pre-read checklist

- [ ] Read full Web Services Agreement at https://abr.business.gov.au/Tools/WebServicesAgreement
- [ ] Fill in Part A sections above
- [ ] Fill in Part B sections above
- [ ] Decide whether library mode and embedded mode carry different obligations
