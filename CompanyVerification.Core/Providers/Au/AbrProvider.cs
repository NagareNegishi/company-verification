namespace CompanyVerification.Core.Providers.Au;

/// <summary>
/// <see cref="IVerificationProvider"/> adapter for the Australian Business Register (ABR).
/// Uses <c>ABRSearchByNameAdvancedSimpleProtocol2017</c> for name search and
/// <c>SearchByABNv202001</c> for entity type lookup.
/// </summary>
public sealed class AbrProvider : VerificationProviderBase
{
    private const string NameSearchUrl =
        "https://abr.business.gov.au/abrxmlsearch/AbrXmlSearch.asmx/ABRSearchByNameAdvancedSimpleProtocol2017";

    private const string AbnLookupUrl =
        "https://abr.business.gov.au/abrxmlsearch/AbrXmlSearch.asmx/SearchByABNv202001";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AbrOptions _options;
    private readonly AbrNameSearchRequest _nameSearch;
    private readonly AbrAbnLookupRequest _abnLookup;

    /// <summary>
    /// Initialises a new instance of <see cref="AbrProvider"/>.
    /// </summary>
    /// <param name="httpClientFactory">Factory used to create HTTP clients.</param>
    /// <param name="options">ABR credentials.</param>
    /// <param name="nameSearch">
    /// Optional name search settings. Pass a configured instance to override defaults.
    /// If <c>null</c>, defaults from <see cref="AbrNameSearchRequest"/> are used.
    /// </param>
    public AbrProvider(
        IHttpClientFactory httpClientFactory,
        AbrOptions options,
        AbrNameSearchRequest? nameSearch = null)
    {
        _httpClientFactory = httpClientFactory;
        _options = options;
        _nameSearch = nameSearch ?? new AbrNameSearchRequest();
        _abnLookup = new AbrAbnLookupRequest();
    }

    /// <inheritdoc/>
    public override IReadOnlyList<string> SupportedCountries { get; } = ["AU"];

    /// <inheritdoc/>
    protected override async Task<IReadOnlyList<CompanyCandidate>> SearchCore(
        string name, string country, CancellationToken cancellationToken)
    {
        var http = _httpClientFactory.CreateClient();

        var qs = _nameSearch.ToQueryString(name, _options.Guid);
        var xml = await http.GetStringAsync($"{NameSearchUrl}?{qs}", cancellationToken);

        var abns = ParseNameSearchAbns(xml);
        if (abns.Count == 0)
            return [];

        // parallel ABN lookups, filter, assemble
        throw new NotImplementedException();
    }

    // -------------------------------------------------------------------------
    // XML parsers
    // -------------------------------------------------------------------------

    /// <summary>
    /// Extracts ABNs from an <c>ABRSearchByNameAdvancedSimpleProtocol2017</c> response.
    /// Returns an empty list if the response contains no <c>searchResultsRecord</c> elements
    /// (e.g. no matches, or ABR returned an <c>exception</c> element instead).
    /// </summary>
    private static IReadOnlyList<string> ParseNameSearchAbns(string xml)
    {
        var doc = XDocument.Parse(xml);

        var records = doc.Descendants()
            .Where(e => e.Name.LocalName == "searchResultsRecord");

        var abns = new List<string>();
        foreach (var record in records)
        {
            var abn = record.Descendants()
                .FirstOrDefault(e => e.Name.LocalName == "identifierValue")
                ?.Value;

            if (!string.IsNullOrWhiteSpace(abn))
                abns.Add(abn);
        }
        return abns;
    }
}
