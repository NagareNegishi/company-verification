namespace CompanyVerification.Core.Providers.Au;

/// <summary>
/// Filter definitions for the ABR adapter. Declares which entity type codes are treated as valid companies.
/// Active filtering is handled server-side via the <c>activeABNsOnly=Y</c> parameter.
/// Override or replace to customise adapter filtering without modifying the adapter.
/// </summary>
public static class AbrFilter
{
    /// <summary>
    /// ABR entity type codes included as valid companies. Any type not in this set is excluded.
    /// IND (Individual/Sole Trader), UIE (Other Unincorporated Entity), PTR, and FPT are
    /// excluded — no separate legal identity.
    /// Codes are 3-letter strings returned by <c>SearchByABNv202001</c>.
    /// A nil or absent <c>entityTypeCode</c> is treated as excluded (not in this set).
    /// </summary>
    /// <remarks>
    /// Source: <see href="https://abr.business.gov.au/Documentation/ReferenceData"/>
    /// </remarks>
    public static readonly IReadOnlySet<string> IncludedEntityTypes = new HashSet<string>
    {
        // Companies
        "PRV", // Australian Private Company
        "PUB", // Australian Public Company
        "OIE", // Other Incorporated Entity
        "COP", // Co-operative

        // Partnerships
        "LPT", // Limited Partnership

        // Trusts
        "TRT", // Other Trust

        // Government entities
        "CGE", // Commonwealth Government Entity
        "SGE", // State Government Entity
        "TGE", // Territory Government Entity
        "LGE", // Local Government Entity
    };
}
