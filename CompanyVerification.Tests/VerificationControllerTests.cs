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
        throw new NotImplementedException();
    }

    [Fact]
    public async Task Search_MatchFound_ReturnsOkWithCandidates()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task Search_NoMatches_ReturnsOkWithEmptyList()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task Search_InvalidInput_ReturnsBadRequest()
    {
        throw new NotImplementedException();
    }
}
