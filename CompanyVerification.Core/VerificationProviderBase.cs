namespace CompanyVerification.Core;

public abstract class VerificationProviderBase : IVerificationProvider
{
    /// <inheritdoc/>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="name"/> or <paramref name="country"/> fail input validation.
    /// </exception>
    public async Task<IReadOnlyList<CompanyCandidate>> Search(
        string name,
        string country,
        CancellationToken cancellationToken = default)
    {
        ValidateName(name);
        ValidateCountry(country);
        return await SearchCore(name.Trim(), country.ToUpperInvariant(), cancellationToken);
    }

    /// <summary>
    /// Registry-specific search. Called by <see cref="Search"/> after validation passes.
    /// Receives a trimmed name and an uppercased country code.
    /// </summary>
    /// <param name="name">Trimmed company name.</param>
    /// <param name="country">Uppercased ISO 3166-1 alpha-2 country code.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    protected abstract Task<IReadOnlyList<CompanyCandidate>> SearchCore(
        string name,
        string country,
        CancellationToken cancellationToken);
}
