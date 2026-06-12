using CompanyVerification.Core.Providers.Au;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;

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
    private static AbrProvider MakeProvider(string nameSearchXml, Func<string, string> abnLookupResolver)
    {
        var handler = new FakeHttpHandler(nameSearchXml, abnLookupResolver);
        var client  = new HttpClient(handler);
        var factory = new StubHttpClientFactory(client);
        return new AbrProvider(factory, Options.Create(new AbrOptions { Guid = "test-guid" }));
    }

    // overload for tests with a single fixed ABN lookup response
    private static AbrProvider MakeProvider(string nameSearchXml, string abnLookupXml)
    {
        var handler = new FakeHttpHandler(nameSearchXml, abnLookupXml);
        // routes all HttpClient calls through the fake handler instead of the network
        var client  = new HttpClient(handler);
        var factory = new StubHttpClientFactory(client);
        return new AbrProvider(factory, Options.Create(new AbrOptions { Guid = "test-guid" }));
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

    [Fact]
    public async Task Search_MultipleAbns_ReturnsAllCandidates()
    {
        // name search returns two ABNs
        var nameSearchXml = """
            <root>
              <searchResultsRecord>
                <ABN><identifierValue>11111111111</identifierValue></ABN>
              </searchResultsRecord>
              <searchResultsRecord>
                <ABN><identifierValue>22222222222</identifierValue></ABN>
              </searchResultsRecord>
            </root>
            """;

        var acme1Xml = """
            <root>
              <businessEntity202001>
                <entityType><entityTypeCode>PRV</entityTypeCode></entityType>
                <mainName><organisationName>Acme Pty Ltd</organisationName></mainName>
              </businessEntity202001>
            </root>
            """;

        var acme2Xml = """
            <root>
              <businessEntity202001>
                <entityType><entityTypeCode>PRV</entityTypeCode></entityType>
                <mainName><organisationName>Acme Holdings Pty Ltd</organisationName></mainName>
              </businessEntity202001>
            </root>
            """;

        // each ABN resolves to a different Acme company
        var provider = MakeProvider(nameSearchXml, abn => abn == "11111111111" ? acme1Xml : acme2Xml);

        var results = await provider.Search("Acme", "AU");

        Assert.Equal(2, results.Count);
        Assert.Contains(results, r => r.RegistryId == "11111111111" && r.Name == "Acme Pty Ltd");
        Assert.Contains(results, r => r.RegistryId == "22222222222" && r.Name == "Acme Holdings Pty Ltd");
    }

    [Fact]
    public async Task Search_MixedEntityTypes_ReturnsOnlyIncluded()
    {
        // name search returns two ABNs — one company, one individual
        var nameSearchXml = """
            <root>
              <searchResultsRecord>
                <ABN><identifierValue>11111111111</identifierValue></ABN>
              </searchResultsRecord>
              <searchResultsRecord>
                <ABN><identifierValue>22222222222</identifierValue></ABN>
              </searchResultsRecord>
            </root>
            """;

        // PRV = private company, included
        var acme1Xml = """
            <root>
              <businessEntity202001>
                <entityType><entityTypeCode>PRV</entityTypeCode></entityType>
                <mainName><organisationName>Acme Pty Ltd</organisationName></mainName>
              </businessEntity202001>
            </root>
            """;

        // IND = Individual/Sole Trader, excluded
        var acme2Xml = """
            <root>
              <businessEntity202001>
                <entityType><entityTypeCode>IND</entityTypeCode></entityType>
                <mainName><organisationName>Acme Trading</organisationName></mainName>
              </businessEntity202001>
            </root>
            """;

        var provider = MakeProvider(nameSearchXml, abn => abn == "11111111111" ? acme1Xml : acme2Xml);

        var results = await provider.Search("Acme", "AU");

        Assert.Single(results);
        Assert.Equal("11111111111", results[0].RegistryId);
    }
}
