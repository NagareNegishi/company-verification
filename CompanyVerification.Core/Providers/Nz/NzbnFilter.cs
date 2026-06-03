namespace CompanyVerification.Core.Providers.Nz;

/// <summary>
/// Filter definitions for the NZBN adapter. Declares which entity status codes
/// and entity type codes are treated as active, verifiable employers.
/// Override or replace to customise adapter filtering without modifying the adapter.
/// </summary>
public static class NzbnFilter
{
    /// <summary>
    /// NZBN entity status codes that map to "active". All other status codes are excluded.
    /// </summary>
    /// <remarks>
    /// Source: <see href="https://github.com/Procuret/nzbn-python/blob/master/nzbn/entity_status.py"/>
    /// </remarks>
    public static readonly IReadOnlySet<string> ActiveStatusCodes = new HashSet<string>
    {
        "50" // Registered
    };

    /// <summary>
    /// NZBN entity type codes included as valid employers. Any type not in this set is excluded.
    /// Sole traders and unincorporated partnerships are excluded — no separate legal identity.
    /// Legacy codes (B, I, D, F, N, S, T, Y, Z, G) are excluded — no official definition found.
    /// </summary>
    /// <remarks>
    /// Source: <see href="https://github.com/Procuret/nzbn-python/blob/master/nzbn/entity_type.py"/>
    /// </remarks>
    public static readonly IReadOnlySet<string> IncludedEntityTypes = new HashSet<string>
    {
        // Companies
        "NZCompany", "LTD", "ULTD", "COOP",
        "OverseasCompany", "ASIC", "NON_ASIC",

        // Partnerships
        "LimitedPartnershipNz", "LimitedPartnershipOverseas",

        // Societies and mutuals
        "IncorporatedSociety", "IndustrialAndProvidentSociety",
        "BuildingSociety", "CreditUnion", "FriendlySociety",

        // Trusts
        "CharitableTrust", "Trust", "Trading_Trust",

        // Statutory and government bodies
        "SpecialBody", "SpecialBodies",
        "GovtCentral", "GovtLocal", "GovtEdu", "GovtOther"
    };
}
