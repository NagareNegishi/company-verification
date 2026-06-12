namespace CompanyVerification.Core.Providers.Au;

/// <summary>
/// Configuration for the ABR adapter. Pass to <see cref="AbrProvider"/> at construction.
/// </summary>
public class AbrOptions
{
    /// <summary>
    /// Authentication GUID issued by ABR on registration.
    /// Each deployer must register their own GUID at
    /// <see href="https://abr.business.gov.au/Documentation/WebServiceRegistration"/>.
    /// Do not commit this value to source control.
    /// </summary>
    public string Guid { get; set; } = string.Empty;

}
