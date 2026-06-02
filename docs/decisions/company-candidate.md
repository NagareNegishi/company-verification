# CompanyCandidate — Design Decisions

## `record` not `class`

Two candidates with the same field values are the same candidate.
`record` gives value-based equality by default; `class` compares by reference.
No methods or behaviour belong here — it is a data carrier only.

## `sealed`

No subtype of a company candidate makes sense. `sealed` communicates that
intent and prevents accidental inheritance.

## `IReadOnlyDictionary<string, string>?` for AdditionalFields

- Interface (`IReadOnlyDictionary`) not concrete type (`Dictionary`): callers
  read only; the adapter owns construction.
- `string` values: registry extra fields are labels, IDs, and dates — all
  representable as strings. Richer types would add complexity with no
  practical benefit at this stage.
- Nullable (`?`) with default `null`: most adapters supply no extra fields.
  Making it optional avoids forcing every adapter to construct an empty
  dictionary. `null` means the adapter chose not to supply extras — it does
  not mean data is unavailable from the registry.
