# ABR API Research — Verified Findings

## Downloadable tool / library

ABR provides Excel spreadsheet tools (macros) for manual use only. No NuGet package, no SDK.

Official sample code: github.com/ABN-SFLookupTechnicalSupport/ABNLookupSampleCode — C# .NET 3.5,
SOAP only. Reference reading; not a dependency.

---

## JSON service

Three endpoints only. Everything else is XML/SOAP.

| Endpoint | Purpose |
|---|---|
| `AbnDetails.aspx?abn=...&guid=...` | Single ABN lookup — returns full entity including `entityTypeCode` |
| `AcnDetails.aspx?acn=...&guid=...` | Single ACN lookup |
| `MatchingNames.aspx?name=...&guid=...` | Name search |

Format: JSONP (`callback({...})`), not plain JSON. Wrapper must be stripped before parsing.

"Limited" means: 3 methods vs ~15 in XML/SOAP, no advanced search filters (postcode, state,
entity type as request params), no pagination.

---

## MatchingNames.aspx — exact response fields

Source: hyraiq/abnlookup PHP library (Name.php model), built directly on this endpoint.

```
abn        string   required
abnStatus  string   required   "Active" | "Cancelled" | "Not Active"
name       string   required
nameType   string   required   "legalName" | "mainName" | "mainTradingName" | ...
postcode   string   required
state      string   required
score      int      required
current    bool     required
```

`entityTypeCode` is absent. Not optional — not present at all.

---

## XML SimpleProtocol (ABRSearchByNameSimpleProtocol) — exact response fields

Source: ABNLookup_schema.xsd — SearchResultsRecord type.

```
ABN[]
  identifierValue   string
  identifierStatus  string   "Active" | "Cancelled" | "Not Active"

[choice, one or more name elements]
  legalName         IndividualSimpleName   → individual entity
  mainName          OrganisationSimpleName → non-individual entity
  mainTradingName   OrganisationSimpleName
  otherTradingName  OrganisationSimpleName
  businessName      OrganisationSimpleName
  (others)

mainBusinessPhysicalAddress[]
```

`entityTypeCode` is absent.

---

## Filtering capability — both endpoints compared

| Signal | XML SimpleProtocol | JSON MatchingNames |
|---|---|---|
| ABN status | `identifierStatus` | `abnStatus` |
| Individual vs non-individual | element name: `legalName` = individual | `nameType` field value |
| Entity type code (PRV, PUB, TRT…) | not available | not available |

Getting entity type code requires a second call to `AbnDetails.aspx` per ABN.

---

## Entity type codes (ABR-specific, 3-letter)

Available only via `AbnDetails.aspx`. Examples confirmed:

`PRV` Australian Private Company — `PUB` Australian Public Company —
`IND` Individual/Sole Trader — `TRT` Trust — `SGE` State Government Entity —
`CGE` Commonwealth Government Entity — `OIE` Other Incorporated Entity

Full list: abr.business.gov.au/Help/EntityTypeList
