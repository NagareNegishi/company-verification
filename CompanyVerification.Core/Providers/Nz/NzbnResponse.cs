using System.Text.Json.Serialization;

namespace CompanyVerification.Core.Providers.Nz;

/// <summary>
/// Maps the NZBN entity search response envelope. Only <see cref="Items"/> is mapped;
/// pagination fields and top-level links are ignored.
/// </summary>
public sealed record NzbnSearchResponse(
    [property: JsonPropertyName("items")]
    IReadOnlyList<NzbnEntity> Items
);

/// <summary>
/// A single entity from an NZBN search response. Maps only the fields required for
/// filtering and constructing a <see cref="CompanyCandidate"/>; all other fields are ignored.
/// </summary>
public sealed record NzbnEntity(
    /// <summary>13-digit New Zealand Business Number.</summary>
    [property: JsonPropertyName("nzbn")]
    string Nzbn,
    /// <summary>Registered legal name of the entity.</summary>
    [property: JsonPropertyName("entityName")]
    string EntityName,
    /// <summary>Entity status code. Compared against <see cref="NzbnFilter.ActiveStatusCodes"/>.</summary>
    [property: JsonPropertyName("entityStatusCode")]
    string EntityStatusCode,
    /// <summary>Entity type code. Compared against <see cref="NzbnFilter.IncludedEntityTypes"/>.</summary>
    [property: JsonPropertyName("entityTypeCode")]
    string EntityTypeCode,
    /// <summary>Name of the source register (e.g. "Companies Register").</summary>
    [property: JsonPropertyName("sourceRegister")]
    string SourceRegister
);
