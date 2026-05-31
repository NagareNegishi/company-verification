# Scaffold — .NET Solution

Documents the one-time commands used to create the solution structure inside the dev container.
Run from the repo root (`/workspaces/company-verification`).

## Solution format

Uses `.slnx` (XML-based solution format) instead of the legacy `.sln` format.
Reasons: human-readable, no GUIDs, clean git diffs.

**In .NET 10, `dotnet new sln` creates `.slnx` by default** — this is a breaking change from .NET 9.
There is no `dotnet new slnx` template. The standard `dotnet new sln` command is all you need.
To get the old format explicitly: `dotnet new sln --format sln`

→ Official (breaking change note): https://learn.microsoft.com/en-us/dotnet/core/compatibility/sdk/10.0/dotnet-new-sln-slnx-default
→ Official (dotnet sln): https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-sln

## Commands

### 1. Create the solution file

```
dotnet new sln -n CompanyVerification
```

Generates `CompanyVerification.slnx` in the current directory (`.NET 10` default).

`-n` — name of the solution file. Without it, defaults to the current directory name
(`company-verification`, kebab-case) — not what we want.

### 2. Scaffold the three projects

```
dotnet new classlib -o CompanyVerification.Core
dotnet new webapi --use-controllers -o CompanyVerification.Api
dotnet new xunit   -o CompanyVerification.Tests
```

`-o` — output directory. Creates the project in a named subdirectory rather than
dumping files into the repo root.

`--use-controllers` — opts into traditional controller-based routing. .NET 8+ defaults
to Minimal APIs (routing defined inline in `Program.cs`). Controllers are chosen here
because the structure is explicit: one class per resource, visible in the file tree.

| Project | Template | Why |
|---|---|---|
| `Core` | `classlib` | No framework dependency — publishable as a NuGet package |
| `Api` | `webapi --use-controllers` | HTTP front door; controllers keep routing explicit |
| `Tests` | `xunit` | Conformance suite + unit tests |

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
