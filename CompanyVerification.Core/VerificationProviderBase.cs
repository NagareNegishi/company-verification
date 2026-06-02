namespace CompanyVerification.Core;

/// <summary>
/// Base class for all country registry adapters. Enforces input validation and
/// delegates registry-specific search to <see cref="SearchCore"/>.
/// </summary>
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

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name must not be null or whitespace.", nameof(name));

        if (name.Length > 200)
            throw new ArgumentException("Name must not exceed 200 characters.", nameof(name));

        foreach (var c in name)
        {
            if (c < 32)
                throw new ArgumentException("Name must not contain control characters.", nameof(name));
            if (c == '<' || c == '>')
                throw new ArgumentException("Name must not contain angle brackets.", nameof(name));
        }
    }

    private static void ValidateCountry(string country)
    {
        if (string.IsNullOrEmpty(country))
            throw new ArgumentException("Country code must not be null or empty.", nameof(country));

        if (country.Length != 2 || !country.All(char.IsLetter))
            throw new ArgumentException("Country code must be a two-letter ISO 3166-1 alpha-2 code.", nameof(country));
    }
}
