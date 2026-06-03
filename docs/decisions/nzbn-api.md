# NZBN API — Verified Findings

All information below is confirmed from official legislation or open-source library source code.
Nothing here is inferred or guessed.

---

## Registration

Register at: `portal.api.business.govt.nz`

Steps:
1. Create an account on the portal
2. Subscribe to the **NZBN API** product — choose the subscription key option (sufficient for public read access; OAuth2 is only needed for write operations)
3. Sign the API Access Agreement when prompted
4. Wait for approval — the support team confirms within one working day
5. Retrieve your **sandbox key** (for development) and **production key** (for live calls) from your account

### Environments

| Environment | Base URL |
|-------------|----------|
| Sandbox | `https://api.business.govt.nz/sandbox/nzbn/v5/` |
| Production | `https://api.business.govt.nz/gateway/nzbn/v5/` |

Sandbox contains a mix of test cases and historical Companies Register data (pre-2010). Use the sandbox key during development; switch to the production key when ready.

Source: [portal.api.business.govt.nz/getting-started](https://portal.api.business.govt.nz/getting-started), [nzbn.govt.nz/using-the-nzbn/nzbn-services/api](https://www.nzbn.govt.nz/using-the-nzbn/nzbn-services/api/)

---

## Endpoint

```
GET https://api.business.govt.nz/gateway/nzbn/v5/entities
```

| Parameter | Type | Required | Value |
|-----------|------|----------|-------|
| `search-term` | query string | yes | company name (partial accepted) |
| `entity-status` | query string | no | `registered` (pre-filters to active only) |
| `page-size` | query string | no | integer, max 200, default 50 |

Source: [t-pearse/nzbn — lib/nzbn.rb](https://github.com/t-pearse/nzbn/blob/master/lib/nzbn.rb)

---

## Authentication

```
Ocp-Apim-Subscription-Key: {api_key}
```

API key obtained by registering at [portal.api.business.govt.nz](https://portal.api.business.govt.nz/api/nzbn).

Source: [t-pearse/nzbn — lib/nzbn.rb](https://github.com/t-pearse/nzbn/blob/master/lib/nzbn.rb)

---

## Response Shape

```json
{
  "items": [
    {
      "nzbn": "9429037626847",
      "entityName": "ACME LIMITED",
      "entityStatusCode": "50",
      "entityTypeCode": "NZCompany",
      "tradingNames": [],
      "classifications": [],
      "registrationDate": "2005-03-01"
    }
  ],
  "totalItems": 1,
  "page": 1,
  "pageSize": 50
}
```

Source: [Procuret/nzbn-python — nzbn/abbreviated_entity.py](https://github.com/Procuret/nzbn-python/blob/master/nzbn/abbreviated_entity.py)

### Field presence guarantees

| Field | Always present | Basis |
|-------|---------------|-------|
| `nzbn` | Yes | Schedule 3 mandatory (NZBN Act 2016) |
| `entityName` | Yes — key always present; value may be `null` for historical records | Schedule 3 mandatory ("Legal entity name"); stored as `Optional` in Procuret library |
| `entityStatusCode` | Yes | Schedule 3 mandatory ("Status"); NZBN Act 2016 |
| `entityTypeCode` | Yes | Schedule 3 mandatory ("Kind of entity"); NZBN Act 2016 |
| `tradingNames` | Yes — but may be empty array | Schedule 3 mandatory ("Trading name or names") |
| `classifications` | No — key may be absent | Schedule 4 optional; Procuret library guards with `if 'classifications' in data` |
| `registrationDate` | Yes — key present; value may be `null` | Schedule 3 mandatory ("Start date"); stored as `Optional` in Procuret library |

**Legislation source:** [NZ Business Number Act 2016 — Schedule 3: Public primary business data](https://www.legislation.govt.nz/act/public/2016/0016/20.0/DLM6431606.html)

Schedule 3 lists the data the register **must** contain for every NZBN entity:
> "Legal entity name, Trading name or names, Registered address, Location identifier, NZBN, Start date, Kind of entity, and Status"

Schedule 4 data (trading areas, industry classification, website, phone, email) is optional — entities choose whether to provide it.

---

## Active Status Code

The adapter must keep only `entityStatusCode == "50"` (REGISTERED).
All other codes represent inactive states.

| Code | Meaning |
|------|---------|
| `50` | Registered ✓ — **keep** |
| `55–66` | Voluntary administration / liquidation / receivership |
| `70–72` | Receivership / statutory administration |
| `80` | Removed |
| `90` | Inactive |
| `91` | Removed — closed |

Source: [Procuret/nzbn-python — nzbn/entity_status.py](https://github.com/Procuret/nzbn-python/blob/master/nzbn/entity_status.py)

---

## Entity Type Codes — Employer Verification Scope

The adapter's purpose is to identify any registered, active NZ entity that can
legitimately offer employment. This is broader than "company" — it includes
government bodies, partnerships with separate legal identity, societies,
co-operatives, and trust structures.

### Keep — confirmed legal entities that can employ

| Code | Entity | Governing legislation / register |
|------|--------|----------------------------------|
| `NZCompany` | NZ limited company | [Companies Act 1993](https://www.legislation.govt.nz/act/public/1993/0105/latest/DLM319576.html) |
| `LTD` | Limited company — variant designation on the same register | [Companies Act 1993](https://www.legislation.govt.nz/act/public/1993/0105/latest/DLM319576.html) |
| `ULTD` | Unlimited company — shareholders carry full liability; rare | [Companies Act 1993](https://www.legislation.govt.nz/act/public/1993/0105/latest/DLM319576.html) |
| `COOP` | Co-operative company — member-owned (e.g. Fonterra, Farmlands) | [Companies Act 1993 Part 3](https://www.legislation.govt.nz/act/public/1993/0105/latest/DLM319576.html) |
| `OverseasCompany` | Foreign company registered to operate in NZ | [Companies Act 1993 Part 18](https://www.legislation.govt.nz/act/public/1993/0105/latest/DLM319576.html) |
| `ASIC` | Australian company (ASIC-regulated) registered in NZ | [Companies Register — overseas companies](https://companies-register.companiesoffice.govt.nz/help-centre/before-you-start-a-company/choosing-a-type-of-company-for-your-business/) |
| `NON_ASIC` | Non-Australian overseas company registered in NZ | [Companies Register — overseas companies](https://companies-register.companiesoffice.govt.nz/help-centre/before-you-start-a-company/choosing-a-type-of-company-for-your-business/) |
| `LimitedPartnershipNz` | NZ limited partnership — used by law firms, accountants, investment funds | [Limited Partnerships Act 2008](https://lp-register.companiesoffice.govt.nz/) |
| `LimitedPartnershipOverseas` | Overseas limited partnership registered in NZ | [Limited Partnerships Act 2008](https://lp-register.companiesoffice.govt.nz/) |
| `IncorporatedSociety` | Member-run non-profit (sports clubs, professional bodies) | [Incorporated Societies Act 2022](https://is-register.companiesoffice.govt.nz/) |
| `IndustrialAndProvidentSociety` | Co-operative for members (e.g. rural supply co-ops) | [Industrial and Provident Societies Act 1908](https://www.companiesoffice.govt.nz/all-registers/other-entities/) |
| `BuildingSociety` | Mutual lender | [Building Societies Act 1965](https://www.companiesoffice.govt.nz/all-registers/other-entities/) |
| `CreditUnion` | Member-owned financial institution | [Friendly Societies and Credit Unions Act 1982](https://www.companiesoffice.govt.nz/all-registers/other-entities/) |
| `FriendlySociety` | Mutual benefit society | [Friendly Societies and Credit Unions Act 1982](https://www.companiesoffice.govt.nz/all-registers/other-entities/) |
| `CharitableTrust` | Charitable trust board — NFP sector | [Charitable Trusts Act 1957 / Charitable Trusts Register](https://ct-register.companiesoffice.govt.nz/) |
| `Trust` | General trust — private arrangement, no dedicated register | No specific register; NZ trusts governed by Trusts Act 2019 |
| `Trading_Trust` | Trust that conducts a business | No specific register; NZ trusts governed by Trusts Act 2019 |
| `SpecialBody` / `SpecialBodies` | Smaller statutory bodies created by specific legislation | [Special and Other Bodies Register](https://www.companiesoffice.govt.nz/all-registers/other-entities/) |
| `GovtCentral` | Central government agencies (Police, IRD, MSD, MBIE, etc.) | [NZBN Act 2016 — public sector entities](https://www.legislation.govt.nz/act/public/2016/0016/latest/whole.html) |
| `GovtLocal` | Local councils | [NZBN Act 2016 — public sector entities](https://www.legislation.govt.nz/act/public/2016/0016/latest/whole.html) |
| `GovtEdu` | Universities, polytechnics, schools | [NZBN Act 2016 — public sector entities](https://www.legislation.govt.nz/act/public/2016/0016/latest/whole.html) |
| `GovtOther` | Crown entities, state-owned enterprises | [NZBN Act 2016 — public sector entities](https://www.legislation.govt.nz/act/public/2016/0016/latest/whole.html) |

### Exclude — with reasons

| Code | Why excluded | Source |
|------|-------------|--------|
| `SoleTrader` / `Sole_Trader` | No separate legal entity — the individual IS the business; nothing to verify as an employer distinct from the person | [NZBN — sole traders](https://www.nzbn.govt.nz/get-an-nzbn/get-your-nzbn/) |
| `Partnership` | No separate legal identity under NZ law — partners trade jointly as individuals | [Partnership Act 1908](https://www.legislation.govt.nz/act/public/1908/0139/latest/whole.html) |
| `B`, `I`, `D`, `F`, `N`, `S`, `T`, `Y`, `Z`, `G` | Legacy or residual codes with no official definition found — cannot confirm what entity they represent or whether they can employ | Observed in [Procuret/nzbn-python — entity_type.py](https://github.com/Procuret/nzbn-python/blob/master/nzbn/entity_type.py); no official documentation located |

Source for full code list: [Procuret/nzbn-python — nzbn/entity_type.py](https://github.com/Procuret/nzbn-python/blob/master/nzbn/entity_type.py)
