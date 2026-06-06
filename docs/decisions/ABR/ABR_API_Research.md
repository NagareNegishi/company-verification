# ABR API Research — Verified Findings

All claims on this page are sourced from official ABR documentation, fetched and verified
2026-06-06. Source URLs are pinpointed pages, not the main site.

---

## Downloadable tool / library

ABR provides Excel spreadsheet tools (macros) for manual use only. No NuGet package, no SDK.

Official sample code: github.com/ABN-SFLookupTechnicalSupport/ABNLookupSampleCode — C# .NET 3.5,
SOAP only. Reference reading; not a dependency.

---

## JSON service

Three endpoints only. Everything else is XML/SOAP (31 operations total).

| Endpoint | Purpose |
|---|---|
| `AbnDetails.aspx?abn=...&guid=...` | Single ABN lookup — returns full entity including `EntityTypeCode` |
| `AcnDetails.aspx?acn=...&guid=...` | Single ACN lookup |
| `MatchingNames.aspx?name=...&guid=...` | Name search |

Format: JSONP (`callback({...})`), not plain JSON. Wrapper must be stripped before parsing.

"Limited" means: 3 endpoints vs 31 XML/SOAP operations, no advanced search filters (postcode,
state, entity type as request params), no pagination.

Source: https://abr.business.gov.au/json/

---

## MatchingNames.aspx — exact response fields

Source: hyraiq/abnlookup PHP library (Name.php model), built directly on this endpoint.
The official JSON page (https://abr.business.gov.au/json/) confirms the endpoint and that
entity type is absent; it does not enumerate individual record fields.

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

## AbnDetails.aspx — exact response fields

Source: https://abr.business.gov.au/json/

```
Abn                    string
Acn                    string
EntityName             string
AbnStatus              string   "Active" | "Cancelled" | "Not Active"
AbnStatusEffectiveFrom string
EntityTypeCode         string   3-letter code (e.g. "PRV", "PUB") — see Reference Data below
EntityTypeName         string   human-readable (e.g. "Australian Private Company")
Gst                    string   GST registration date or null
AddressDate            string
AddressPostcode        string
AddressState           string
BusinessName           array
Message                string   error or info; empty on success
```

---

## XML name search — exact response fields

Two relevant operations. Both return `SearchResultsRecord` per result.

| Operation | Active-only filter |
|---|---|
| `ABRSearchByNameSimpleProtocol` | No server-side filter; client must filter by `identifierStatus` |
| `ABRSearchByNameAdvancedSimpleProtocol2017` | Accepts `activeABNsOnly=Y` — filters server-side |

**Prefer `ABRSearchByNameAdvancedSimpleProtocol2017`** to avoid fetching cancelled ABNs.

`SearchResultsRecord` fields (source: `ABNLookup_schema.xsd`, confirmed by
https://abr.business.gov.au/Documentation/WebServiceResponse and
https://abr.business.gov.au/Documentation/WebServiceMethods):

```
ABN[]
  identifierValue   string   11-digit ABN
  identifierStatus  string   "Active" | "Cancelled" | "Not Active"

[choice, one or more name elements]
  legalName         IndividualSimpleName   → individual entity
  mainName          OrganisationSimpleName → non-individual entity
  mainTradingName   OrganisationSimpleName
  otherTradingName  OrganisationSimpleName
  businessName      OrganisationSimpleName
  (others)

mainBusinessPhysicalAddress[]
  stateCode         string
  postcode          string
  isCurrentIndicator string
```

`entityTypeCode` is absent. Not optional — not present at all.

Source: https://abr.business.gov.au/Documentation/WebServiceMethods
> "None [of the name search methods] return entity type data in search results."

---

## XML ABN detail lookup — exact response fields

Operation: `SearchByABNv202001` (latest version, recommended)

Returns `ResponseBusinessEntity202001`. Relevant fields:

```
entityType
  entityTypeCode        string(4)   3-letter code (e.g. "PRV")
  entityDescription     string      human-readable

entityStatus[]
  entityStatusCode      string      status value
  effectiveFrom         date
  effectiveTo           date

ABN, ASICNumber, names, addresses, GST, charity, ACNC, superannuation, AWEF
```

Source: https://abr.business.gov.au/Documentation/DataDictionary

---

## Filtering capability — both endpoints compared

| Signal | XML `ABRSearchByNameAdvancedSimpleProtocol2017` | JSON `MatchingNames.aspx` |
|---|---|---|
| ABN status | `identifierStatus`; or use `activeABNsOnly=Y` server-side | `abnStatus` field |
| Individual vs non-individual | element name: `legalName` = individual | `nameType` field value |
| Entity type code | **not available** — requires second call | **not available** — requires second call |

Getting entity type code requires a second call to `SearchByABNv202001` (XML) or
`AbnDetails.aspx` (JSON) per ABN.

Source: https://abr.business.gov.au/Documentation/WebServiceMethods
Source: https://abr.business.gov.au/Documentation/WebServiceResponse

---

## Entity type codes — ABR-specific, 3-letter format

**Correction from earlier research:** `abr.business.gov.au/Help/EntityTypeList` uses
**numeric IDs** (`?Id=19`) for the website UI, not API codes. The 3-letter codes used by
the API are at `abr.business.gov.au/Documentation/ReferenceData`.

Source: https://abr.business.gov.au/Documentation/ReferenceData

Selected codes relevant to company verification:

| Code | Description |
|---|---|
| `PRV` | Australian Private Company |
| `PUB` | Australian Public Company |
| `IND` | Individual/Sole Trader |
| `TRT` | Other Trust |
| `OIE` | Other Incorporated Entity |
| `UIE` | Other Unincorporated Entity |
| `COP` | Co-operative |
| `LPT` | Limited Partnership |
| `PTR` | Other Partnership |
| `FPT` | Family Partnership |
| `CGE` | Commonwealth Government Entity |
| `SGE` | State Government Entity |
| `TGE` | Territory Government Entity |
| `LGE` | Local Government Entity |

Full list (150+ codes) at: https://abr.business.gov.au/Documentation/ReferenceData

---

## All XML/SOAP operations

31 operations total. Source: https://abr.business.gov.au/abrxmlsearch/AbrXmlSearch.asmx

Name search group (use `ABRSearchByNameAdvancedSimpleProtocol2017`):
`ABRSearchByNameSimpleProtocol`, `ABRSearchByNameAdvancedSimpleProtocol`,
`ABRSearchByNameAdvancedSimpleProtocol2006`, `ABRSearchByNameAdvancedSimpleProtocol2012`,
`ABRSearchByNameAdvancedSimpleProtocol2017`, `ABRSearchByNameAdvancedSimpleProtocol2023`,
`ABRSearchByName`, `ABRSearchByNameAdvanced`, `ABRSearchByNameAdvanced2006`,
`ABRSearchByNameAdvanced2012`, `ABRSearchByNameAdvanced2017`, `ABRSearchByNameAdvanced2023`

ABN lookup group (use `SearchByABNv202001`):
`ABRSearchByABN`, `SearchByABNv200506`, `SearchByABNv200709`, `SearchByABNv201205`,
`SearchByABNv201408`, `SearchByABNv202001`

---

## Adapter design decisions

### Call strategy

Two calls per search. XML throughout — no JSON.

1. `ABRSearchByNameAdvancedSimpleProtocol2017` with `activeABNsOnly=Y`
   Returns `SearchResultsRecord`: ABN + status, name elements, address. No entity type.
   `activeABNsOnly=Y` filters cancelled ABNs server-side.

2. `SearchByABNv202001` per ABN from step 1
   Returns `ResponseBusinessEntity202001`: includes `entityType > entityTypeCode` (3-letter string).
   Entity type can be nil or absent — see null handling below.

JSON rejected: JSONP format, no `activeABNsOnly` filter, would require mixed-format parsing
alongside the XML detail call.

### Null entity type handling

`entityType` in `SearchByABNv202001` response is `nillable="true"` (XSD). `entityTypeCode`
inside it is `minOccurs="0"`. Either can be absent.

Decision: configurable, default exclude.
- Default (exclude): nil or absent `entityTypeCode` → entity not returned. Safe — rejects
  unclassifiable entities rather than letting unknowns through.
- Opt-in (include): nil or absent `entityTypeCode` → entity returned without type filtering.
  For callers who need maximum recall and accept the risk.

No other free official API provides entity type for Australian businesses. ASIC has no public
API — only third-party paid wrappers exist. ABR second call is the only reliable free source.
