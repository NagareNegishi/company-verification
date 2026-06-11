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
}
