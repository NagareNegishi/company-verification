# Adding a Country Adapter

## Checklist

**Core**
- [ ] Create `CompanyVerification.Core/Providers/{CC}/` where `{CC}` is the ISO 3166-1 alpha-2 code
- [ ] Add `{Cc}Options.cs` — credentials class, `string` properties with `set` and `string.Empty` defaults
- [ ] Add `{Cc}Provider.cs` — extends `VerificationProviderBase`, declares `SupportedCountries = ["{CC}"]`
- [ ] Add `{Cc}Filter.cs` — declares which status codes and entity types are included
- [ ] Add `conformance.yaml` — documents the filter decisions

**Wiring**
- [ ] Add a property to `CompanyVerificationOptions` for the new options
- [ ] Add `services.Configure<{Cc}Options>` and `services.AddSingleton<IVerificationProvider, {Cc}Provider>()` to `ServiceCollectionExtensions.cs`
- [ ] Add a warning check in `OptionsWarningService` for the new credential

**Infrastructure**
- [ ] Add the registry hostname to `.devcontainer/project-firewall.sh`

**Tests**
- [ ] Add `{Cc}ProviderTests.cs` covering active company, excluded entity type, inactive status, empty results, cancelled token, multiple entities, mixed types
