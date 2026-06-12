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
}
