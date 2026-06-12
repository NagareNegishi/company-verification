# NuGet Publishing Research

Source: [Package authoring best practices](https://learn.microsoft.com/en-us/nuget/create-packages/package-authoring-best-practices)

## Package metadata fields

All values to set in `CompanyVerification.Core.csproj`:

| Property | Value | Notes |
|---|---|---|
| `PackageId` | `CompanyVerification.Core` | |
| `Version` | `0.1.0-alpha` | Pre-release: the DO rule says publish as pre-release if not stable |
| `Authors` | `Nagare Negishi` | "pretty name", not username |
| `Description` | `Given a company name and country, returns whether the company is legally active. Includes adapters for New Zealand (NZBN) and Australia (ABR).` | |
| `Copyright` | `Copyright (c) Nagare Negishi 2026` | |
| `PackageLicenseExpression` | `AGPL-3.0-only` | SPDX expression; FSF-approved so NuGet accepts it |
| `PackageReadmeFile` | `README.md` | Bundles existing README into the package |
| `PackageProjectUrl` | `https://github.com/NagareNegishi/company-verification` | |
| `RepositoryUrl` | `https://github.com/NagareNegishi/company-verification` | |
| `RepositoryType` | `git` | |
| `PackageTags` | `company verification nz au nzbn abr business registry` | |
| `PackageReleaseNotes` | `Initial release.` | |

## Do later

- `PackageIcon` — add a 128x128 transparent PNG asset, then set `PackageIcon` in the `.csproj`.
- Source Link — add `Microsoft.SourceLink.GitHub` package reference; auto-populates `RepositoryUrl` and per-commit metadata.
- `PackageVersion` via `Version` prefix reservation — no nuget.org account set up yet.

## DO NOT

- `LicenseUrl` — deprecated, do not use.
- `IconUrl` — deprecated, do not use.

## DI extension method

The .NET convention for library authors: extension method on `IServiceCollection` in the `Microsoft.Extensions.DependencyInjection` namespace, named `AddX`, returns `IServiceCollection` for chaining. Options are passed as `Action<TOptions>` so callers configure inline or bind from `IConfiguration`.

Source: [Options pattern guidance for .NET library authors](https://learn.microsoft.com/en-us/dotnet/core/extensions/options-library-authors)

## Decision: one method or one per adapter

```csharp
// Option A: combined
services.AddCompanyVerification(
    abr: o => o.Guid = "...",
    nzbn: o => o.SubscriptionKey = "...");

// Option B: per adapter
services.AddAbrProvider(o => o.Guid = "...");
services.AddNzbnProvider(o => o.SubscriptionKey = "...");
```

Option B follows how ASP.NET Core composes features (e.g. `AddAuthentication().AddJwtBearer()`). It lets a consumer use only one adapter without pulling in the other's options. Option A is simpler when both adapters are always used together.

Given the project is MVP with exactly two adapters always deployed together, Option A is the simpler starting point.
