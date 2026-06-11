using CompanyVerification.Core.Providers.Au;
using Microsoft.Extensions.Http;

namespace CompanyVerification.Tests;

public sealed class AbrProviderTests
{
    // Returns the same pre-wired HttpClient for every CreateClient() call.
    private sealed class StubHttpClientFactory : IHttpClientFactory
    {
        private readonly HttpClient _client;
        public StubHttpClientFactory(HttpClient client) => _client = client;
        public HttpClient CreateClient(string name) => _client;
    }

    // Builds an AbrProvider backed by FakeHttpHandler so tests never hit the real ABR API.
    private static AbrProvider MakeProvider(string nameSearchXml, string abnLookupXml)
    {
        var handler = new FakeHttpHandler(nameSearchXml, abnLookupXml);
        // routes all HttpClient calls through the fake handler instead of the network
        var client  = new HttpClient(handler);
        var factory = new StubHttpClientFactory(client);
        return new AbrProvider(factory, new AbrOptions { Guid = "test-guid" });
    }

    [Fact]
    public async Task Search_ActiveCompany_ReturnsCandidate()
    {
        // name search returns one ABN
        var nameSearchXml = """
            <root>
              <searchResultsRecord>
                <ABN><identifierValue>12345678901</identifierValue></ABN>
              </searchResultsRecord>
            </root>
            """;

        // ABN lookup returns a private company with an included entity type
        var abnLookupXml = """
            <root>
              <businessEntity202001>
                <entityType><entityTypeCode>PRV</entityTypeCode></entityType>
                <mainName><organisationName>Acme Pty Ltd</organisationName></mainName>
              </businessEntity202001>
            </root>
            """;

        var provider = MakeProvider(nameSearchXml, abnLookupXml);
        var results  = await provider.Search("Acme", "AU");

        Assert.Single(results);
        Assert.Equal("12345678901", results[0].RegistryId);
        Assert.Equal("Acme Pty Ltd",  results[0].Name);
        Assert.Equal("AU",            results[0].Country);
    }

    [Fact]
    public async Task Search_ExcludedEntityType_ReturnsEmpty()
    {
        var nameSearchXml = """
            <root>
              <searchResultsRecord>
                <ABN><identifierValue>12345678901</identifierValue></ABN>
              </searchResultsRecord>
            </root>
            """;

        // IND = Individual/Sole Trader — excluded from AbrFilter.IncludedEntityTypes
        var abnLookupXml = """
            <root>
              <businessEntity202001>
                <entityType><entityTypeCode>IND</entityTypeCode></entityType>
                <mainName><organisationName>John Smith</organisationName></mainName>
              </businessEntity202001>
            </root>
            """;

        var provider = MakeProvider(nameSearchXml, abnLookupXml);
        var results  = await provider.Search("John", "AU");

        Assert.Empty(results);
    }

    [Fact]
    public async Task Search_NoAbnsFromNameSearch_ReturnsEmpty()
    {
        // name search returns no records — no ABNs to look up
        var nameSearchXml = """
            <root></root>
            """;

        var provider = MakeProvider(nameSearchXml, abnLookupXml: "");
        var results  = await provider.Search("Unknown Co", "AU");

        Assert.Empty(results);
    }

    [Fact]
    public async Task Search_AbnLookupMissingEntity_ReturnsEmpty()
    {
        var nameSearchXml = """
            <root>
              <searchResultsRecord>
                <ABN><identifierValue>12345678901</identifierValue></ABN>
              </searchResultsRecord>
            </root>
            """;

        // ABR returns an error element instead of businessEntity202001
        var abnLookupXml = """
            <root>
              <exception>
                <exceptionCode>SearchUnavailable</exceptionCode>
              </exception>
            </root>
            """;

        var provider = MakeProvider(nameSearchXml, abnLookupXml);
        var results  = await provider.Search("Acme", "AU");

        Assert.Empty(results);
    }

    [Fact]
    public async Task Search_CancelledToken_Throws()
    {
        var provider = MakeProvider(nameSearchXml: "", abnLookupXml: "");

        // token is already cancelled before Search is called
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => provider.Search("Acme", "AU", cts.Token));
    }
}
