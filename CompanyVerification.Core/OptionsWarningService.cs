using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CompanyVerification.Core.Providers.Au;
using CompanyVerification.Core.Providers.Nz;

namespace CompanyVerification.Core;

/// <summary>
/// Logs a warning at startup for any adapter whose credentials are missing or empty.
/// The app continues to run; searches for the unconfigured country will fail at request time.
/// </summary>
internal sealed class OptionsWarningService : IHostedService
{
    private readonly IOptions<AbrOptions> _abr;
    private readonly IOptions<NzbnOptions> _nzbn;
    private readonly ILogger<OptionsWarningService> _logger;

    /// <summary>
    /// Initialises a new instance of <see cref="OptionsWarningService"/>.
    /// </summary>
    /// <param name="abr">ABR adapter options.</param>
    /// <param name="nzbn">NZBN adapter options.</param>
    /// <param name="logger">Logger for startup warnings.</param>
    public OptionsWarningService(
        IOptions<AbrOptions> abr,
        IOptions<NzbnOptions> nzbn,
        ILogger<OptionsWarningService> logger)
    {
        _abr = abr;
        _nzbn = nzbn;
        _logger = logger;
    }

    /// <inheritdoc/>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_abr.Value.Guid))
            _logger.LogWarning("ABR__Guid is not configured. AU searches will fail.");

        if (string.IsNullOrWhiteSpace(_nzbn.Value.SubscriptionKey))
            _logger.LogWarning("NZBN__SubscriptionKey is not configured. NZ searches will fail.");

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
