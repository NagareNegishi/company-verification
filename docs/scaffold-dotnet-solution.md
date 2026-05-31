# Scaffold — .NET Solution

Documents the one-time commands used to create the solution structure inside the dev container.
Run from the repo root (`/workspaces/company-verification`).

## Solution format

Uses `.slnx` (XML-based solution format, .NET 10+) instead of the legacy `.sln` format.
Reasons: human-readable, no GUIDs, clean git diffs.
→ Official: https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-new-sdk-templates#slnx

## Commands

### 1. Create the solution file

```
dotnet new slnx -n CompanyVerification
```

Generates `CompanyVerification.slnx` in the current directory.

### 2. Scaffold the three projects

```
dotnet new classlib -o CompanyVerification.Core
dotnet new webapi --use-controllers -o CompanyVerification.Api
dotnet new xunit   -o CompanyVerification.Tests
```

| Project | Template | Why |
|---|---|---|
| `Core` | `classlib` | No framework dependency — publishable as a NuGet package |
| `Api` | `webapi --use-controllers` | HTTP front door; controllers keep routing explicit |
| `Tests` | `xunit` | Conformance suite + unit tests |

`--use-controllers` opt-in: .NET 8+ defaults to Minimal API style.
Controllers are chosen here because the routing structure is explicit and familiar for a learning context.
→ Official (webapi): https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api
→ Official (classlib): https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-new-sdk-templates#classlib
→ Official (xunit): https://xunit.net/docs/getting-started/v3/cmdline

### 3. Register projects in the solution

```
dotnet sln add CompanyVerification.Core
dotnet sln add CompanyVerification.Api
dotnet sln add CompanyVerification.Tests
```

Adds each `.csproj` path into `CompanyVerification.slnx` so IDEs and `dotnet build` at the root see all three.
→ Official: https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-sln

### 4. Wire project references

```
dotnet add CompanyVerification.Api     reference CompanyVerification.Core
dotnet add CompanyVerification.Tests   reference CompanyVerification.Core
```

Tells the compiler that `Api` and `Tests` depend on `Core`.
`Api` does NOT reference `Tests` — tests are never a runtime dependency.
→ Official: https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-add-reference

## Verify

```
dotnet build
```

All three projects should build with zero errors on a fresh scaffold.

## Result

```
CompanyVerification.slnx
CompanyVerification.Core/
CompanyVerification.Api/
CompanyVerification.Tests/
```
