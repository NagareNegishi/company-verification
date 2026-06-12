using CompanyVerification.Api.Controllers;
using CompanyVerification.Core;
using Microsoft.AspNetCore.Mvc;

namespace CompanyVerification.Tests;

public sealed class VerificationControllerTests
{
    // set SupportedCountries and SearchResult per test.
    private sealed class StubProvider : IVerificationProvider
    {
        public IReadOnlyList<string> SupportedCountries { get; init; } = [];

        // returns candidates or throws ArgumentException.
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
        var controller = new VerificationController(
            [new StubProvider
            {
                SupportedCountries = ["AU"],
                SearchResult = () => throw new ArgumentException("Name must not exceed 200 characters.")
            }]);

        var result = await controller.Search(new string('A', 201), "AU", CancellationToken.None);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("200 characters", bad.Value?.ToString());
    }
}
