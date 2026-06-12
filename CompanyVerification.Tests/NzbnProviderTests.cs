using CompanyVerification.Core.Providers.Nz;
using Microsoft.Extensions.Options;

namespace CompanyVerification.Tests;

public sealed class NzbnProviderTests
{
    // Returns the same pre-wired HttpClient for every CreateClient() call.
    private sealed class StubHttpClientFactory : IHttpClientFactory
    {
        private readonly HttpClient _client;
        public StubHttpClientFactory(HttpClient client) => _client = client;
        public HttpClient CreateClient(string name) => _client;
    }

    // Builds an NzbnProvider backed by FakeHttpHandler so tests never hit the real NZBN API.
    private static NzbnProvider MakeProvider(string json)
    {
        var handler = new FakeHttpHandler(json);
        var client  = new HttpClient(handler);
        var factory = new StubHttpClientFactory(client);
        return new NzbnProvider(factory, Options.Create(new NzbnOptions { SubscriptionKey = "test-key" }));
    }

    [Fact]
    public async Task Search_ActiveCompany_ReturnsCandidate()
    {
        // entity search returns one registered company
        var json = """
            {
              "items": [
                {
                  "nzbn": "9429041234567",
                  "entityName": "Acme Limited",
                  "entityStatusCode": "50",
                  "entityTypeCode": "NZCompany"
                }
              ]
            }
            """;

        var provider = MakeProvider(json);
        var results  = await provider.Search("Acme", "NZ");

        Assert.Single(results);
        Assert.Equal("9429041234567", results[0].RegistryId);
        Assert.Equal("Acme Limited",  results[0].Name);
        Assert.Equal("NZ",            results[0].Country);
    }

    [Fact]
    public async Task Search_ExcludedEntityType_ReturnsEmpty()
    {
        // SoleTrader = excluded — no separate legal identity
        var json = """
            {
              "items": [
                {
                  "nzbn": "9429041234567",
                  "entityName": "John Smith",
                  "entityStatusCode": "50",
                  "entityTypeCode": "SoleTrader"
                }
              ]
            }
            """;

        var provider = MakeProvider(json);
        var results  = await provider.Search("John", "NZ");

        Assert.Empty(results);
    }
}
