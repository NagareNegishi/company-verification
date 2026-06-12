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
    /// <param name="configureAbr">Configures <see cref="AbrOptions"/>. Set <c>Guid</c> from <c>ABR__Guid</c>.</param>
    /// <param name="configureNzbn">Configures <see cref="NzbnOptions"/>. Set <c>SubscriptionKey</c> from <c>NZBN__SubscriptionKey</c>.</param>
    public static IServiceCollection AddCompanyVerification(
        this IServiceCollection services,
        Action<AbrOptions> configureAbr,
        Action<NzbnOptions> configureNzbn)
    {
        services.Configure(configureAbr);
        services.Configure(configureNzbn);
        services.AddHttpClient();
        services.AddSingleton<IVerificationProvider, AbrProvider>();
        services.AddSingleton<IVerificationProvider, NzbnProvider>();
        services.AddHostedService<OptionsWarningService>();
        return services;
    }
}
