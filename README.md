# company-verification

A small, focused service that answers one question: **given a company name and a country, does this company exist and is it currently active?**

Returns a normalized result regardless of which country's official government registry the data came from. Pluggable adapters — one per country, no changes to core logic when adding a new one.

## What "company" means

Built-in adapters return any **registered, active legal entity that can
legitimately post jobs** — not only companies in the strict corporate sense.
The New Zealand adapter, for example, includes limited companies, government
bodies, incorporated societies, charitable trusts, co-operatives, and limited
partnerships. Sole traders and unincorporated partnerships are excluded because
they have no separate legal identity.

Adapters written by others define their own inclusion logic. What qualifies as
"active" and which entity types are included is each adapter's responsibility,
declared in its conformance file. This is by design — filter rules are
registry-specific and context-specific.

## Usage

Intended for distribution as a NuGet package. The same core can also be consumed
as an HTTP API or, optionally, as an MCP server — all three are thin wrappers
over a shared core with no business logic in the transport layer.

## Status

Early development. MVP targets New Zealand (NZBN) natively; Australia and other countries via a configurable fallback provider.

## Stack

C# / .NET 10 — class library, HTTP API, and optionally an MCP server front door over a shared core.

## Licence

[AGPL-3.0](LICENSE) — free for non-commercial use. Commercial use requires a separate licence from the project owner.