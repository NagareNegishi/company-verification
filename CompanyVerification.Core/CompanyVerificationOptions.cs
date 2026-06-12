using CompanyVerification.Core.Providers.Au;
using CompanyVerification.Core.Providers.Nz;

namespace CompanyVerification.Core;

/// <summary>
/// Top-level options for <see cref="CompanyVerificationServiceCollectionExtensions.AddCompanyVerification"/>.
/// Add a property here when registering a new country adapter.
/// </summary>
public sealed class CompanyVerificationOptions
{
    /// <summary>Australian Business Register adapter options.</summary>
    public AbrOptions Abr { get; } = new();

    /// <summary>New Zealand Business Number adapter options.</summary>
    public NzbnOptions Nzbn { get; } = new();
}
