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
}
