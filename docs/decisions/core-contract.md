# Core Contract — Design Decisions

## Interface + abstract base class, not just one or the other

The interface (`IVerificationProvider`) and the base class (`VerificationProviderBase`)
serve different roles:

- The interface is what consumers (controller, tests) depend on. It defines the shape.
- The base class is where enforcement lives — validation guaranteed to run before any
  adapter touches the input.

An interface alone cannot run code. An abstract class alone cannot be mocked freely
in tests or wired into .NET dependency injection without coupling callers to the
inheritance chain.

## `SearchCore` naming

The Template Method pattern requires a separate override point so adapters cannot
bypass validation by overriding `Search()` directly. `SearchCore` follows the
established .NET convention (e.g. `ExecuteCore`, `ValidateCore`) — recognisable to
.NET developers without explanation.

## Validation is private, not on the interface

`ValidateName` and `ValidateCountry` are `private static` on the base class.
Putting them on the interface would force every mock and test double to implement
them, and imply the interface enforces validation — which it cannot.

## Normalisation happens in `Search()`, not in the validation methods

`name.Trim()` and `country.ToUpperInvariant()` run in `Search()` after validation
passes. Validation checks the raw input; normalisation produces the clean input for
`SearchCore`. Keeping these separate makes each step's responsibility clear.

## `IsNullOrEmpty` for country, `IsNullOrWhiteSpace` for name

A country code of `"  "` (spaces) should fail the length check (`!= 2`), not be
silently swallowed as empty. `IsNullOrWhiteSpace` would mask that. For name,
whitespace-only is meaningless as a search term, so `IsNullOrWhiteSpace` is correct.

## `SupportedCountries` on the interface, not just the base class

The routing layer works against `IVerificationProvider` and must be able to ask any
adapter "which countries do you handle?" without knowing its concrete type.

`IReadOnlyList<string>` rather than `string` so an adapter covering multiple
jurisdictions does not need a separate class per country. Single-country adapters
return a one-element list — no cost.

Declared `abstract` on `VerificationProviderBase` so the compiler rejects any adapter
that omits it. A missing country declaration is a routing bug; it should not compile.
