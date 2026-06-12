using System.Net.Http.Json;
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
        var http = _httpClientFactory.CreateClient();
        http.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _options.SubscriptionKey);

        var url = $"{BaseUrl}/entities?search-term={Uri.EscapeDataString(name)}&entity-status=registered&page-size={PageSize}";

        var response = await http.GetFromJsonAsync<NzbnSearchResponse>(url, cancellationToken);
        if (response is null)
            return [];

        return response.Items
            .Where(e => NzbnFilter.ActiveStatusCodes.Contains(e.EntityStatusCode))
            .Where(e => NzbnFilter.IncludedEntityTypes.Contains(e.EntityTypeCode))
            .Select(e => new CompanyCandidate(e.Nzbn, e.EntityName, "NZ"))
            .ToList();
    }
}
