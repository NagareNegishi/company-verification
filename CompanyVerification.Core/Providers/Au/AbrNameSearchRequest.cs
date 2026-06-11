namespace CompanyVerification.Core.Providers.Au;

/// <summary>
/// Configurable settings for an <c>ABRSearchByNameAdvancedSimpleProtocol2017</c> HTTP GET request.
/// Construct once and reuse across searches. Call <see cref="ToQueryString"/> per search,
/// passing the name and GUID at call time.
/// </summary>
public sealed class AbrNameSearchRequest
{
    /// <summary>
    /// Postcode filter against the main business location. Empty string means no filter.
    /// </summary>
    public string Postcode { get; init; } = "";

    /// <summary>Search in legal names. Default: <c>true</c>.</summary>
    public bool LegalName { get; init; } = true;

    /// <summary>
    /// Search in trading names. ABR trading name data has not been updated since 2012.
    /// Default: <c>false</c>.
    /// </summary>
    public bool TradingName { get; init; } = false;

    /// <summary>Search in business names. Default: <c>false</c>.</summary>
    public bool BusinessName { get; init; } = false;

    /// <summary>Restrict results to active ABNs only. Default: <c>true</c>.</summary>
    public bool ActiveABNsOnly { get; init; } = true;

    /// <summary>
    /// State filters. All <c>false</c> means national search — no state restriction.
    /// </summary>
    public bool NSW { get; init; } = false;
    /// <inheritdoc cref="NSW"/>
    public bool SA  { get; init; } = false;
    /// <inheritdoc cref="NSW"/>
    public bool ACT { get; init; } = false;
    /// <inheritdoc cref="NSW"/>
    public bool VIC { get; init; } = false;
    /// <inheritdoc cref="NSW"/>
    public bool WA  { get; init; } = false;
    /// <inheritdoc cref="NSW"/>
    public bool NT  { get; init; } = false;
    /// <inheritdoc cref="NSW"/>
    public bool QLD { get; init; } = false;
    /// <inheritdoc cref="NSW"/>
    public bool TAS { get; init; } = false;

    /// <summary>
    /// Search strategy. Valid values: <c>"Typical"</c>, <c>"Narrow"</c>. Default: <c>"Typical"</c>.
    /// </summary>
    public string SearchWidth { get; init; } = "Typical";

    /// <summary>
    /// Minimum relevance score for results. Accepted range: 50–100. Default: 60.
    /// </summary>
    public int MinimumScore { get; init; } = 60;

    /// <summary>
    /// Maximum number of results to return. The API has no hard cap; its own default is 200.
    /// Default: 30.
    /// </summary>
    public int MaxSearchResults { get; init; } = 30;

    /// <summary>
    /// Builds the query string for the ABR name search HTTP GET request.
    /// </summary>
    /// <param name="name">The company name to search for.</param>
    /// <param name="guid">ABR authentication GUID from <see cref="AbrOptions.Guid"/>.</param>
    public string ToQueryString(string name, string guid) =>
        $"name={Uri.EscapeDataString(name)}" +
        $"&postcode={Uri.EscapeDataString(Postcode)}" +
        $"&legalName={YN(LegalName)}&tradingName={YN(TradingName)}&businessName={YN(BusinessName)}" +
        $"&activeABNsOnly={YN(ActiveABNsOnly)}" +
        $"&NSW={YN(NSW)}&SA={YN(SA)}&ACT={YN(ACT)}&VIC={YN(VIC)}" +
        $"&WA={YN(WA)}&NT={YN(NT)}&QLD={YN(QLD)}&TAS={YN(TAS)}" +
        $"&authenticationGuid={Uri.EscapeDataString(guid)}" +
        $"&searchWidth={Uri.EscapeDataString(SearchWidth)}" +
        $"&minimumScore={MinimumScore}" +
        $"&maxSearchResults={MaxSearchResults}";

    /// <summary>Converts a boolean to the <c>"Y"</c>/<c>"N"</c> string the ABR API expects.</summary>
    private static string YN(bool value) => value ? "Y" : "N";
}
