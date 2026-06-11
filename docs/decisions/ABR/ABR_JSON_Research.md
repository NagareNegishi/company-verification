# ABR API Research — JSON Endpoints

Not the chosen implementation path. Recorded for reference only.
See ABR_XML_Research.md for the chosen approach.

Verified 2026-06-06.

---

## Why JSON was not chosen

1. **No `activeABNsOnly` filter.** `MatchingNames.aspx` returns cancelled entities and the
   client must filter them out. The XML 2017 variant filters server-side.

2. **JSONP format.** Responses are `callback({...})` wrappers, not plain JSON. The wrapper
   must be stripped before parsing. XML with `XDocument` is cleaner.

3. **Mixed-format problem.** Getting entity type requires a second call to `SearchByABNv202001`
   (XML only, no JSON equivalent). Using JSON for the name search creates a JSONP + XML
   mixed parsing path.

---

## JSON service overview

Three endpoints only. Everything else is XML/SOAP.

Source: https://abr.business.gov.au/json/

| Endpoint | Purpose |
|---|---|
| `AbnDetails.aspx?abn=...&guid=...` | Single ABN lookup, has `EntityTypeCode` |
| `AcnDetails.aspx?acn=...&guid=...` | Single ACN lookup |
| `MatchingNames.aspx?name=...&guid=...` | Name search, no `EntityTypeCode` |

Format: JSONP (`callback({...})`), not plain JSON.

---

## MatchingNames.aspx — response fields

Source: hyraiq/abnlookup PHP library (Name.php), built directly on this endpoint.
Official docs (https://abr.business.gov.au/json/) confirm entity type is absent but do not
enumerate individual record fields.

```
abn        string   required
abnStatus  string   required
name       string   required
nameType   string   required   "legalName" | "mainName" | "mainTradingName" | ...
postcode   string   required
state      string   required
score      int      required
current    bool     required
```

`entityTypeCode` is not in this response.

`nameType == "legalName"` means the result is an individual entity. That is the only type
signal available here without a second call.

---

## AbnDetails.aspx — response fields

Source: https://abr.business.gov.au/json/

```
Abn                    string
Acn                    string
EntityName             string
AbnStatus              string
AbnStatusEffectiveFrom string
EntityTypeCode         string   3-letter code (e.g. "PRV", "PUB")
EntityTypeName         string   human-readable (e.g. "Australian Private Company")
Gst                    string
AddressDate            string
AddressPostcode        string
AddressState           string
BusinessName           array
Message                string
```

`EntityTypeCode` is present. Whether it can be null is not confirmed by official docs.
For the 3-letter code list, see the entity type codes section in ABR_XML_Research.md.
