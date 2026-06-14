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
                  "entityTypeCode": "NZCompany",
                  "sourceRegister": "Companies Register"
                }
              ]
            }
            """;

        var provider = MakeProvider(json);
        var results  = await provider.Search("Acme", "NZ");

        Assert.Single(results);
        Assert.Equal("9429041234567",      results[0].RegistryId);
        Assert.Equal("Acme Limited",       results[0].Name);
        Assert.Equal("NZ",                 results[0].Country);
        Assert.Equal("Companies Register", results[0].AdditionalFields!["source_register"]);
        Assert.True(results[0].AdditionalFields!.ContainsKey("searched_at"));
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

    [Fact]
    public async Task Search_InactiveStatusCode_ReturnsEmpty()
    {
        // status code "10" = removed — not in NzbnFilter.ActiveStatusCodes
        var json = """
            {
              "items": [
                {
                  "nzbn": "9429041234567",
                  "entityName": "Defunct Limited",
                  "entityStatusCode": "10",
                  "entityTypeCode": "NZCompany"
                }
              ]
            }
            """;

        var provider = MakeProvider(json);
        var results  = await provider.Search("Defunct", "NZ");

        Assert.Empty(results);
    }

    [Fact]
    public async Task Search_EmptyItems_ReturnsEmpty()
    {
        // entity search returns no matches
        var json = """
            {
              "items": []
            }
            """;

        var provider = MakeProvider(json);
        var results  = await provider.Search("Unknown Co", "NZ");

        Assert.Empty(results);
    }

    [Fact]
    public async Task Search_CancelledToken_Throws()
    {
        var provider = MakeProvider(json: "");

        // token is already cancelled before Search is called
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => provider.Search("Acme", "NZ", cts.Token));
    }

    [Fact]
    public async Task Search_MultipleEntities_ReturnsAllCandidates()
    {
        // entity search returns two registered companies
        var json = """
            {
              "items": [
                {
                  "nzbn": "9429041234567",
                  "entityName": "Acme Limited",
                  "entityStatusCode": "50",
                  "entityTypeCode": "NZCompany"
                },
                {
                  "nzbn": "9429049876543",
                  "entityName": "Acme Holdings Limited",
                  "entityStatusCode": "50",
                  "entityTypeCode": "NZCompany"
                }
              ]
            }
            """;

        var provider = MakeProvider(json);
        var results  = await provider.Search("Acme", "NZ");

        Assert.Equal(2, results.Count);
        Assert.Contains(results, r => r.RegistryId == "9429041234567" && r.Name == "Acme Limited");
        Assert.Contains(results, r => r.RegistryId == "9429049876543" && r.Name == "Acme Holdings Limited");
    }

    [Fact]
    public async Task Search_MixedEntityTypes_ReturnsOnlyIncluded()
    {
        // two registered entities — one company, one sole trader
        var json = """
            {
              "items": [
                {
                  "nzbn": "9429041234567",
                  "entityName": "Acme Limited",
                  "entityStatusCode": "50",
                  "entityTypeCode": "NZCompany"
                },
                {
                  "nzbn": "9429049876543",
                  "entityName": "Acme Trading",
                  "entityStatusCode": "50",
                  "entityTypeCode": "SoleTrader"
                }
              ]
            }
            """;

        var provider = MakeProvider(json);
        var results  = await provider.Search("Acme", "NZ");

        Assert.Single(results);
        Assert.Equal("9429041234567", results[0].RegistryId);
    }
}
