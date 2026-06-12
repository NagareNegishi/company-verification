using System.Xml.Linq;
using Microsoft.Extensions.Options;

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
        IOptions<AbrOptions> options,
        AbrNameSearchRequest? nameSearch = null)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
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

        // ABR name search returns no entity type
        var abns = ParseNameSearchAbns(xml);
        if (abns.Count == 0)
            return [];

        // second call to retrieve entity type for filtering
        var tasks = abns.Select(async abn =>
        {
            var abnXml = await http.GetStringAsync(
                $"{AbnLookupUrl}?{_abnLookup.ToQueryString(abn, _options.Guid)}", cancellationToken);
            return ParseAbnLookupResult(abnXml, abn);
        });

        var results = await Task.WhenAll(tasks);
        // drop nulls
        return results.OfType<CompanyCandidate>().ToList();
    }


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


    /// <summary>
    /// Parses a <c>SearchByABNv202001</c> response and returns a <see cref="CompanyCandidate"/>
    /// if the entity passes type filtering, or <c>null</c> if it should be excluded.
    /// A nil or absent <c>entityTypeCode</c> returns <c>null</c>; unclassifiable entities are excluded by default.
    /// </summary>
    private static CompanyCandidate? ParseAbnLookupResult(string xml, string abn)
    {
        var doc = XDocument.Parse(xml);
        var entity = doc.Descendants()
            .FirstOrDefault(e => e.Name.LocalName == "businessEntity202001");

        // redundant given activeABNsOnly=Y, but guards against an ABR error response on a single lookup
        if (entity == null)
            return null;

        var entityTypeCode = entity
            .Descendants().FirstOrDefault(e => e.Name.LocalName == "entityType")
            ?.Descendants().FirstOrDefault(e => e.Name.LocalName == "entityTypeCode")
            ?.Value;

        // unclassifiable entities are excluded by default
        if (string.IsNullOrWhiteSpace(entityTypeCode))
            return null;

        // entity type not in the included set
        if (!AbrFilter.IncludedEntityTypes.Contains(entityTypeCode))
            return null;

        var name = entity
            .Descendants().FirstOrDefault(e => e.Name.LocalName == "mainName")
            ?.Descendants().FirstOrDefault(e => e.Name.LocalName == "organisationName")
            ?.Value;

        // guards against malformed ABR data
        if (string.IsNullOrWhiteSpace(name))
            return null;

        return new CompanyCandidate(abn, name, "AU");
    }


}
