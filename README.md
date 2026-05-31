# company-verification

A small, focused service that answers one question: **given a company name and a country, does this company exist and is it currently active?**

Returns a normalized result regardless of which country's official government registry the data came from. Pluggable adapters — one per country, no changes to core logic when adding a new one.

## Status

Early development. MVP targets New Zealand (NZBN) natively; Australia and other countries via a configurable fallback provider.

## Stack

C# / .NET 10 — class library, HTTP API, and optionally an MCP server front door over a shared core.

## Licence

[AGPL-3.0](LICENSE) — free for non-commercial use. Commercial use requires a separate licence from the project owner.