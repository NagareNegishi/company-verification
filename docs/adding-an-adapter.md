# Adding a Country Adapter

## Checklist

**Core**
- [ ] Create `CompanyVerification.Core/Providers/{CC}/` where `{CC}` is the ISO 3166-1 alpha-2 code
- [ ] Add `{Cc}Options.cs` — credentials class, `string` properties with `set` and `string.Empty` defaults
- [ ] Add `{Cc}Provider.cs` — extends `VerificationProviderBase`, declares `SupportedCountries = ["{CC}"]`
- [ ] Add `{Cc}Filter.cs` — declares which status codes and entity types are included
- [ ] Add `conformance.yaml` — documents the filter decisions

## conformance.yaml schema

```yaml
adapter: <lowercase identifier>
country: <ISO 3166-1 alpha-2>

# Omit if the registry API filters server-side (e.g. activeABNsOnly=Y).
# Required otherwise — list the registry's exact status strings.
active_statuses:
  - "<exact status string>"

# Always required. List the registry's exact entity type strings.
included_entity_types:
  - "<exact type string>"

# Always required. Empty block if the adapter sets no extra fields.
additional_fields:
  <field_name>:
    source: api_response | adapter
    type: <.NET type>
    format: <value format, if relevant>
    description: <what the value contains>
    required_by: <compliance clause, if applicable>
```

See `Providers/Nz/conformance.yaml` and `Providers/Au/conformance.yaml` for complete examples.

**Wiring**
- [ ] Add a property to `CompanyVerificationOptions` for the new options
- [ ] Add `services.Configure<{Cc}Options>` and `services.AddSingleton<IVerificationProvider, {Cc}Provider>()` to `ServiceCollectionExtensions.cs`
- [ ] Add a warning check in `OptionsWarningService` for the new credential

**Infrastructure**
- [ ] Add the registry hostname to `.devcontainer/project-firewall.sh`

**Tests**
- [ ] Add `{Cc}ProviderTests.cs` covering active company, excluded entity type, inactive status, empty results, cancelled token, multiple entities, mixed types
