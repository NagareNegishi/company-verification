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

    [Fact]
    public async Task Search_WhitespaceName_Throws() =>
        await Assert.ThrowsAsync<ArgumentException>(() => _provider.Search("   ", "AU"));

    [Fact]
    public async Task Search_NameTooLong_Throws() =>
        await Assert.ThrowsAsync<ArgumentException>(() => _provider.Search(new string('a', 201), "AU"));

    [Fact]
    public async Task Search_NameWithControlCharacter_Throws() =>
        await Assert.ThrowsAsync<ArgumentException>(() => _provider.Search("Acme\x01Corp", "AU"));

    [Fact]
    public async Task Search_NameWithAngleBracket_Throws() =>
        await Assert.ThrowsAsync<ArgumentException>(() => _provider.Search("Acme<Corp", "AU"));

    // ── country validation ───────────────────────────────────────────────────

    [Fact]
    public async Task Search_NullCountry_Throws() =>
        await Assert.ThrowsAsync<ArgumentException>(() => _provider.Search("Acme", null!));

    [Fact]
    public async Task Search_EmptyCountry_Throws() =>
        await Assert.ThrowsAsync<ArgumentException>(() => _provider.Search("Acme", ""));

    [Theory]
    [InlineData("A")]
    [InlineData("AUS")]
    public async Task Search_WrongLengthCountry_Throws(string country) =>
        await Assert.ThrowsAsync<ArgumentException>(() => _provider.Search("Acme", country));

    // ── normalisation ────────────────────────────────────────────────────────
}
