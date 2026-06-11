using CompanyVerification.Core;

namespace CompanyVerification.Tests;

public sealed class VerificationProviderBaseTests
{
    // Minimal concrete subclass to make VerificationProviderBase instantiable.
    private sealed class StubProvider : VerificationProviderBase
    {
        public override IReadOnlyList<string> SupportedCountries => ["AU"];
        public string? CapturedName    { get; private set; }
        public string? CapturedCountry { get; private set; }

        protected override Task<IReadOnlyList<CompanyCandidate>> SearchCore(
            string name, string country, CancellationToken cancellationToken)
        {
            CapturedName    = name;
            CapturedCountry = country;
            return Task.FromResult<IReadOnlyList<CompanyCandidate>>([]);
        }
    }

    private readonly StubProvider _provider = new();

    // ── name validation ──────────────────────────────────────────────────────

    [Fact]
    public async Task Search_NullName_Throws() =>
        await Assert.ThrowsAsync<ArgumentException>(() => _provider.Search(null!, "AU"));

    // ── country validation ───────────────────────────────────────────────────

    // ── normalisation ────────────────────────────────────────────────────────
}
