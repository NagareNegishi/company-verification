namespace CompanyVerification.Core.Providers.Au;

/// <summary>
/// Builds the query string for a <c>SearchByABNv202001</c> HTTP GET request.
/// </summary>
public sealed class AbrAbnLookupRequest
{
    /// <summary>
    /// Builds the query string for the ABR ABN lookup HTTP GET request.
    /// </summary>
    /// <param name="abn">11-digit ABN to look up.</param>
    /// <param name="guid">ABR authentication GUID from <see cref="AbrOptions.Guid"/>.</param>
    public string ToQueryString(string abn, string guid) =>
        // includeHistoricalDetails is always N — verification requires current status only.
        $"searchString={abn}" +
        $"&includeHistoricalDetails=N" +
        $"&authenticationGuid={Uri.EscapeDataString(guid)}";
}
