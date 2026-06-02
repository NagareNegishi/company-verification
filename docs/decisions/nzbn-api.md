# NZBN API — Verified Findings

All information below is confirmed from open-source library source code.
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
