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

## Decision: combined options object

A single `Action<CompanyVerificationOptions>` parameter. Each adapter gets a nested property on `CompanyVerificationOptions` (`Abr`, `Nzbn`, ...). Adding a new adapter adds one property — the call site stays the same shape.

```csharp
services.AddCompanyVerification(o =>
{
    o.Abr.Guid = "...";
    o.Nzbn.SubscriptionKey = "...";
});
```

Per-adapter methods (`AddAbrProvider`, `AddNzbnProvider`) were considered but rejected: they require callers to remember each method as adapters grow, and they don't scale cleanly past two or three adapters.
