namespace CompanyVerification.Core;

/// <summary>
/// A company returned by a registry search that is active and of a company type.
/// </summary>
/// <param name="RegistryId">
/// The registry-native identifier. Format is registry-specific
/// (e.g. 13-digit NZBN for New Zealand, 11-digit ABN for Australia).
/// </param>
/// <param name="Name">The registered legal name of the company.</param>
/// <param name="Country">
/// ISO 3166-1 alpha-2 country code for the registry this result came from (e.g. <c>"NZ"</c>, <c>"AU"</c>).
/// </param>
/// <param name="AdditionalFields">
/// Optional registry-specific fields as key-value pairs. Content is the adapter's
/// responsibility. <c>null</c> means the adapter supplies no extra data.
/// </param>
public sealed record CompanyCandidate(
    string RegistryId,
    string Name,
    string Country,
    IReadOnlyDictionary<string, string>? AdditionalFields = null
);
