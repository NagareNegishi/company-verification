using System.Net;
using System.Text;

namespace CompanyVerification.Tests;

/// <summary>
/// Fake HTTP handler for ABR adapter tests. Intercepts outgoing HTTP calls and
/// returns preset XML based on the URL path. Throws for any URL not recognised.
/// </summary>
internal sealed class FakeHttpHandler : DelegatingHandler
{
    private readonly string? _nameSearchXml;
    private readonly string? _abnLookupXml;
    private readonly Func<string, string>? _abnLookupResolver;

    /// <param name="nameSearchXml">
    /// XML to return for <c>ABRSearchByNameAdvancedSimpleProtocol2017</c> requests.
    /// </param>
    /// <param name="abnLookupXml">
    /// XML to return for all <c>SearchByABNv202001</c> requests.
    /// </param>
    public FakeHttpHandler(string nameSearchXml, string abnLookupXml)
    {
        _nameSearchXml = nameSearchXml;
        _abnLookupXml  = abnLookupXml;
    }

    /// <param name="nameSearchXml">
    /// XML to return for <c>ABRSearchByNameAdvancedSimpleProtocol2017</c> requests.
    /// </param>
    /// <param name="abnLookupResolver">
    /// Returns XML for a <c>SearchByABNv202001</c> request given the ABN being looked up.
    /// Use when different ABNs need different responses.
    /// </param>
    public FakeHttpHandler(string nameSearchXml, Func<string, string> abnLookupResolver)
    {
        _nameSearchXml     = nameSearchXml;
        _abnLookupResolver = abnLookupResolver;
    }

    /// <inheritdoc/>
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // throws before any URL check if the token is already cancelled
        cancellationToken.ThrowIfCancellationRequested();

        var url = request.RequestUri!.ToString();

        // dispatch to the matching preset response based on which ABR endpoint was called
        if (url.Contains("ABRSearchByNameAdvancedSimpleProtocol2017"))
            return Task.FromResult(XmlResponse(_nameSearchXml!));

        if (url.Contains("SearchByABNv202001"))
        {
            // use per-ABN resolver if provided, otherwise return the same XML for all lookups
            var xml = _abnLookupResolver != null
                ? _abnLookupResolver(ExtractAbn(url))
                : _abnLookupXml!;
            return Task.FromResult(XmlResponse(xml));
        }

        // any unrecognised URL is a bug in the adapter, not a test failure to swallow silently
        throw new InvalidOperationException($"Unexpected URL: {url}");
    }

    // extracts the ABN from the searchString query parameter
    private static string ExtractAbn(string url)
    {
        var start = url.IndexOf("searchString=") + "searchString=".Length;
        var end   = url.IndexOf('&', start);
        return end == -1 ? url[start..] : url[start..end];
    }

    private static HttpResponseMessage XmlResponse(string xml) =>
        new(HttpStatusCode.OK)
        {
            Content = new StringContent(xml, Encoding.UTF8, "text/xml")
        };
}
