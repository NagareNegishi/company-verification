# company-verification

A small, focused service that answers one question: **given a company name and a country, does this company exist and is it currently active?**

Returns a normalized result regardless of which country's registry the data came from. Each country has its own adapter; adding a new one never requires changes to core logic.

## What "company" means

Built-in adapters return any **registered, active legal entity that can legitimately post jobs**, not just companies in the corporate sense. The New Zealand adapter includes limited companies, government bodies, incorporated societies, charitable trusts, co-operatives, and limited partnerships. Sole traders and unincorporated partnerships are excluded because they have no separate legal identity.

Adapters written by others define their own inclusion logic. What qualifies as "active" and which entity types are included is each adapter's responsibility, declared in its conformance file. Filter rules are registry-specific and intentionally left to each adapter.

## Usage

Available as a NuGet package. The same core can also be consumed as an HTTP API or an MCP server.

## NZBN API credentials

To use the New Zealand adapter you must supply your own NZBN subscription key.
The key used during development of this library cannot be shared or redistributed.

Steps:
1. Register at [portal.api.business.govt.nz](https://portal.api.business.govt.nz)
2. Sign the MBIE API Access Agreement
3. Set the key in your environment as `NZBN_SUBSCRIPTION_KEY`

Never commit your key to source control.

## Status

Early development. New Zealand (NZBN) is the primary target. Australia (ABR) is planned as a native adapter. Countries without a native adapter return "unsupported".

## Stack

C# / .NET 10, structured as a class library, HTTP API, and MCP server over a shared core.

## Licence

[AGPL-3.0](LICENSE). Free for non-commercial use. Commercial use requires a separate licence from the project owner.
