namespace CompanyVerification.Core;

/// <summary>
/// Contract that every country registry adapter must implement.
/// </summary>
public interface IVerificationProvider
{
    /// <summary>
    /// Searches a country registry for active companies matching the given name.
    /// Returns only active entities of a company type; filtering is the adapter's
    /// responsibility before returning.
    /// </summary>
    /// <param name="name">
    /// The company name to search for. Partial names are accepted; matching is
    /// case-insensitive and handled by the registry.
    /// </param>
    /// <param name="country">
    /// ISO 3166-1 alpha-2 country code (e.g. <c>"NZ"</c>, <c>"AU"</c>).
    /// Accepted case-insensitively.
    /// </param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// Matching candidates, or an empty list if none were found.
    /// An empty list is a valid result, not an error.
    /// </returns>
    Task<IReadOnlyList<CompanyCandidate>> Search(
        string name,
        string country,
        CancellationToken cancellationToken = default);
}
