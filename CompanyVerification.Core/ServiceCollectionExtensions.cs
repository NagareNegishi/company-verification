using CompanyVerification.Core;
using CompanyVerification.Core.Providers.Au;
using CompanyVerification.Core.Providers.Nz;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for registering CompanyVerification services with <see cref="IServiceCollection"/>.
/// </summary>
public static class CompanyVerificationServiceCollectionExtensions
{
    /// <summary>
    /// Registers both country adapters, HTTP client support, and startup credential warnings.
    /// </summary>
    /// <param name="services">The service collection to add to.</param>
    /// <param name="configure">Configures credentials for all registered adapters.</param>
    public static IServiceCollection AddCompanyVerification(
        this IServiceCollection services,
        Action<CompanyVerificationOptions> configure)
    {
        var opts = new CompanyVerificationOptions();
        configure(opts);

        services.Configure<AbrOptions>(o => o.Guid = opts.Abr.Guid);
        services.Configure<NzbnOptions>(o => o.SubscriptionKey = opts.Nzbn.SubscriptionKey);
        services.AddHttpClient();
        services.AddSingleton<IVerificationProvider, AbrProvider>();
        services.AddSingleton<IVerificationProvider, NzbnProvider>();
        services.AddHostedService<OptionsWarningService>();
        return services;
    }
}
