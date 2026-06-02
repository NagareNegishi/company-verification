# NZBN API — Verified Findings

All information below is confirmed from official legislation or open-source library source code.
Nothing here is inferred or guessed.

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

## Company Entity Type Codes

The adapter must keep only these two `entityTypeCode` values.
All others (trusts, sole traders, partnerships, govt bodies, societies) are not companies.

| Code | Meaning |
|------|---------|
| `NZCompany` | New Zealand limited company ✓ — **keep** |
| `OverseasCompany` | Overseas company registered in NZ ✓ — **keep** |

Source: [Procuret/nzbn-python — nzbn/entity_type.py](https://github.com/Procuret/nzbn-python/blob/master/nzbn/entity_type.py)
