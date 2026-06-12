using Microsoft.Extensions.Options;

namespace CompanyVerification.Core.Providers.Nz;

/// <summary>
/// <see cref="IVerificationProvider"/> adapter for the New Zealand Business Number (NZBN) register.
/// </summary>
public sealed class NzbnProvider : VerificationProviderBase
{
    private const string BaseUrl = "https://api.business.govt.nz/gateway/nzbn/v5";
    private const int PageSize = 30;

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly NzbnOptions _options;

    /// <summary>
    /// Initialises a new instance of <see cref="NzbnProvider"/>.
    /// </summary>
    /// <param name="httpClientFactory">Factory used to create HTTP clients.</param>
    /// <param name="options">NZBN credentials.</param>
    public NzbnProvider(IHttpClientFactory httpClientFactory, IOptions<NzbnOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
    }

    /// <inheritdoc/>
    public override IReadOnlyList<string> SupportedCountries { get; } = ["NZ"];

    /// <inheritdoc/>
    protected override async Task<IReadOnlyList<CompanyCandidate>> SearchCore(
        string name, string country, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
