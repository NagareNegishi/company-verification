using CompanyVerification.Api.Controllers;
using CompanyVerification.Core;
using Microsoft.AspNetCore.Mvc;

namespace CompanyVerification.Tests;

public sealed class VerificationControllerTests
{
    private sealed class StubProvider : IVerificationProvider
    {
        public IReadOnlyList<string> SupportedCountries { get; init; } = [];
        public Func<Task<IReadOnlyList<CompanyCandidate>>> SearchResult { get; init; } =
            () => Task.FromResult<IReadOnlyList<CompanyCandidate>>([]);

        public Task<IReadOnlyList<CompanyCandidate>> Search(
            string name, string country, CancellationToken cancellationToken = default)
            => SearchResult();
    }

    [Fact]
    public async Task Search_UnsupportedCountry_ReturnsNotFound()
    {
        var controller = new VerificationController(
            [new StubProvider { SupportedCountries = ["AU"] }]);

        var result = await controller.Search("Acme", "XX", CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Search_MatchFound_ReturnsOkWithCandidates()
    {
        var candidate = new CompanyCandidate("12345678901", "Acme Pty Ltd", "AU");

        var controller = new VerificationController(
            [new StubProvider
            {
                SupportedCountries = ["AU"],
                SearchResult = () => Task.FromResult<IReadOnlyList<CompanyCandidate>>([candidate])
            }]);

        var result = await controller.Search("Acme", "AU", CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        var results = Assert.IsAssignableFrom<IReadOnlyList<CompanyCandidate>>(ok.Value);
        Assert.Single(results);
        Assert.Equal("12345678901", results[0].RegistryId);
    }

    [Fact]
    public async Task Search_NoMatches_ReturnsOkWithEmptyList()
    {
        var controller = new VerificationController(
            [new StubProvider { SupportedCountries = ["AU"] }]);

        var result = await controller.Search("Unknown Co", "AU", CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        var results = Assert.IsAssignableFrom<IReadOnlyList<CompanyCandidate>>(ok.Value);
        Assert.Empty(results);
    }

    [Fact]
    public async Task Search_InvalidInput_ReturnsBadRequest()
    {
        throw new NotImplementedException();
    }
}
