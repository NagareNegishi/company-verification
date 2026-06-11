using System.Net;
using System.Text;

namespace CompanyVerification.Tests;

/// <summary>
/// Fake HTTP handler for ABR adapter tests. Intercepts outgoing HTTP calls and
/// returns preset XML based on the URL path. Throws for any URL not recognised —
/// prevents silently swallowing unexpected calls.
/// </summary>
internal sealed class FakeHttpHandler : DelegatingHandler
{
    private readonly string _nameSearchXml;
    private readonly string _abnLookupXml;

    /// <param name="nameSearchXml">
    /// XML to return for <c>ABRSearchByNameAdvancedSimpleProtocol2017</c> requests.
    /// </param>
    /// <param name="abnLookupXml">
    /// XML to return for <c>SearchByABNv202001</c> requests.
    /// </param>
    public FakeHttpHandler(string nameSearchXml, string abnLookupXml)
    {
        _nameSearchXml = nameSearchXml;
        _abnLookupXml  = abnLookupXml;
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
            return Task.FromResult(XmlResponse(_nameSearchXml));

        if (url.Contains("SearchByABNv202001"))
            return Task.FromResult(XmlResponse(_abnLookupXml));

        // any unrecognised URL is a bug in the adapter, not a test failure to swallow silently
        throw new InvalidOperationException($"Unexpected URL: {url}");
    }

    private static HttpResponseMessage XmlResponse(string xml) =>
        new(HttpStatusCode.OK)
        {
            Content = new StringContent(xml, Encoding.UTF8, "text/xml")
        };
}
