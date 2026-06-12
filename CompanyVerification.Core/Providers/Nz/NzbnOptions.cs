namespace CompanyVerification.Core.Providers.Nz;

/// <summary>
/// Configuration for the NZBN adapter. Pass to <see cref="NzbnProvider"/> at construction.
/// </summary>
public class NzbnOptions
{
    /// <summary>
    /// Subscription key issued by the NZBN API portal on registration.
    /// Each deployer must register their own key at
    /// <see href="https://portal.api.business.govt.nz"/>.
    /// Do not commit this value to source control.
    /// </summary>
    public required string SubscriptionKey { get; init; }
}
