# ABR API Research — XML Web Services

Chosen implementation path. All claims sourced from official ABR documentation,
verified 2026-06-06. Source links go to specific pages, not the main site.

See ABR_JSON_Research.md for JSON endpoint findings (not chosen).

No NuGet package or SDK exists for the ABR API. Implementation uses raw HTTP GET with XML parsing.

---

## Name search — ABRSearchByNameAdvancedSimpleProtocol2017

Five Search by Name versions exist. Each has a SOAP-only variant and a SimpleProtocol
variant (HTTP GET/POST). Use `ABRSearchByNameAdvancedSimpleProtocol2017`. It is the
latest, covers all Advanced2012 options, and adds `activeABNsOnly` to filter
cancelled ABNs server-side.

Source: https://abr.business.gov.au/Documentation/WebServiceMethods

Returns `SearchResultsRecord` per result:

```
ABN[]
  identifierValue      string   11-digit ABN
  identifierStatus     string   xs:string; no enum defined in schema

[choice, one or more name elements]
  legalName            IndividualSimpleName   individual entity
  mainName             OrganisationSimpleName non-individual entity
  mainTradingName      OrganisationSimpleName
  otherTradingName     OrganisationSimpleName
  businessName         OrganisationSimpleName
  (others)

mainBusinessPhysicalAddress[]
  stateCode            string
  postcode             string
  isCurrentIndicator   string
```

`entityTypeCode` is not in this response.

Source: https://abr.business.gov.au/Documentation/WebServiceMethods
> "None of the name search methods return entity type data in search results."
Source: `ABNLookup_schema.xsd` — `SearchResultsRecord` type

### HTTP GET parameters (SimpleProtocol)

Source: WSDL — `ABRSearchByNameAdvancedSimpleProtocol2017HttpGetIn`

All three nameType flags must be present. Omitting any returns HTTP 500 "Missing parameter: X".
They are Y/N flags declaring which name field to search — `name` is the search term, not any of these.

| Parameter | Use | Notes |
|---|---|---|
| `name` | search string | |
| `postcode` | empty | optional filter |
| `legalName` | `Y` | search in legal names |
| `tradingName` | `N` | data not updated since 2012 |
| `businessName` | `N` | |
| `activeABNsOnly` | `Y` | server-side active filter |
| `NSW` `SA` `ACT` `VIC` `WA` `NT` `QLD` `TAS` | `N` | all must be present; `N` = national search |
| `authenticationGuid` | GUID | not `guid` |
| `searchWidth` | `Typical` | valid values: `Typical`, `Narrow` |
| `minimumScore` | `60` | official docs: accepted range is 50–100 (positive integers); `0` is not valid |
| `maxSearchResults` | configurable | no hard cap; API default is 200; see AbrOptions |

---

## ABN detail — SearchByABNv202001

Second call per ABN to retrieve entity type. Use this version over older variants.

Returns `ResponseBusinessEntity202001`. Relevant fields:

```
entityType                            nillable="true" in XSD; wrapper element can be nil
  entityTypeCode   string(4)          3-letter code (e.g. "PRV"), minOccurs="0", can be absent
  entityDescription  string           human-readable description

entityStatus[]
  entityStatusCode   string
  effectiveFrom      date
  effectiveTo        date

ABN, ASICNumber, names, addresses, GST, charity, ACNC, superannuation, AWEF
```

Source: https://abr.business.gov.au/Documentation/DataDictionary
Source: `ABNLookup_schema.xsd` — `ResponseBusinessEntity202001` type

---

## Entity type codes — 3-letter format

Used in the `SearchByABNv202001` response `entityTypeCode` field.

`abr.business.gov.au/Help/EntityTypeList` uses numeric IDs (`?Id=19`) for the website UI.
Those are not the API codes. The 3-letter codes below are what the API returns.

Source: https://abr.business.gov.au/Documentation/ReferenceData

Selected codes relevant to company verification:

| Code | Description |
|---|---|
| `PRV` | Australian Private Company |
| `PUB` | Australian Public Company |
| `IND` | Individual/Sole Trader |
| `OIE` | Other Incorporated Entity |
| `UIE` | Other Unincorporated Entity |
| `COP` | Co-operative |
| `LPT` | Limited Partnership |
| `PTR` | Other Partnership |
| `FPT` | Family Partnership |
| `TRT` | Other Trust |
| `CGE` | Commonwealth Government Entity |
| `SGE` | State Government Entity |
| `TGE` | Territory Government Entity |
| `LGE` | Local Government Entity |

Full list (150+ codes): https://abr.business.gov.au/Documentation/ReferenceData

---

## Adapter design decisions

### Call strategy

Two calls per search, XML throughout.

1. `ABRSearchByNameAdvancedSimpleProtocol2017` with `activeABNsOnly=Y`
   Returns `SearchResultsRecord`: ABN, status, name elements, address. No entity type.

2. `SearchByABNv202001` per ABN from step 1
   Returns `ResponseBusinessEntity202001` with `entityType > entityTypeCode`.
   Entity type can be nil or absent (see null handling below).

### Null entity type handling

`entityType` in `SearchByABNv202001` is `nillable="true"` in the XSD.
`entityTypeCode` inside it is `minOccurs="0"`. Either can be absent.

Decision: configurable, default exclude.

- Default (exclude): nil or absent `entityTypeCode` means the entity is not returned.
  Rejects unclassifiable entities rather than letting unknowns through.
- Opt-in (include): nil or absent `entityTypeCode` means the entity is returned without
  type filtering. For callers who need maximum recall and accept the risk.

ASIC has no public API. Only third-party paid wrappers exist. The ABR second call is the
only free official source of entity type for Australian businesses.

### Configurable search parameters

`searchWidth`, `minimumScore`, and `maxSearchResults` are configurable with no hard API cap.
They are properties on `AbrNameSearchRequest` with defaults: `searchWidth=Typical`, `minimumScore=60`,
`maxSearchResults=30`. `AbrOptions` holds only the authentication GUID.

`maxSearchResults=30` is a conservative default. Higher values increase result count but also increase
the number of parallel call-2 requests.

### Class design

- `AbrOptions` — GUID credential only. One instance per deployment.
- `AbrNameSearchRequest` — call-1 settings object. Constructed once, reused per search.
  `name` and `guid` are passed to `ToQueryString(name, guid)` at call time, not stored.
- `AbrAbnLookupRequest` — no configurable properties. `includeHistoricalDetails` is always `N`;
  verification requires current status only. `abn` and `guid` passed to `ToQueryString` at call time.
- Base URLs are constants in `AbrProvider`. The ABR endpoints do not vary by environment.

### XML parsing

Uses `System.Xml.Linq` (`XDocument`, `XElement`). No NuGet package required.

ABR responses may include an XML namespace on the root element. Using `.Elements("name")` silently
fails when a namespace is present — `e.Name == "name"` only matches the no-namespace form.
Decision: navigate with `.Descendants().Where(e => e.Name.LocalName == "name")` throughout.
`LocalName` strips the namespace prefix and matches regardless.

Name and entity type are both taken from call-2 (`businessEntity202001`). Call-1 parsing extracts
ABNs only. This keeps the call-1 parser minimal and avoids duplicating name extraction logic.

Source: https://learn.microsoft.com/en-us/dotnet/api/system.xml.linq

### Implementation progress

Steps complete: `AbrOptions`, `AbrNameSearchRequest`, `AbrAbnLookupRequest`, `AbrProvider` skeleton,
call-1 HTTP + ABN extraction (`ParseNameSearchAbns`).

Remaining: call-2 parallel lookup (`Task.WhenAll`, `ParseAbnLookupResult`), filter + assemble
(`AbrFilter`, build `CompanyCandidate`), unit tests.

### HTTP GET parameters (SimpleProtocol)

`searchString` (ABN, 11 digits), `includeHistoricalDetails` (`N`), `authenticationGuid` (note: not `guid`).
