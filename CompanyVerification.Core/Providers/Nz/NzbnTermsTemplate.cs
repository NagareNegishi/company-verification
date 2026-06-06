namespace CompanyVerification.Core.Providers.Nz;

/// <summary>
/// Clause to include in your product's signup terms when displaying NZBN data to users.
/// Required by clause 4.11 of the MBIE API Access Agreement (November 2022).
/// Item 6 is this project's own caveat, not an obligation from the agreement.
/// </summary>
public static class NzbnTermsTemplate
{
    public static readonly string UserAgreementClause = """
        Use of New Zealand business register data
        This service uses business information sourced from registers administered
        by the New Zealand Ministry of Business, Innovation & Employment (MBIE).
        By using this service you agree that:
        1. You will not use this data in a way that is contrary to any relevant
           legislation, including the Privacy Act 2020.
        2. You will not use or display this data on any website or service that:
           a. incites hatred whether based on race, religion, gender, sexuality or
              otherwise, or promotes, encourages or facilitates anti-social behaviour;
           b. promotes, encourages or facilitates violence;
           c. promotes, encourages or facilitates terrorism or other activities that
              risk New Zealand national security;
           d. discriminates against any specific social group or otherwise exploits
              vulnerable sections of society;
           e. promotes, facilitates or encourages illegal activity;
           f. is misleading, pornographic, defamatory, or contains illegal or
              otherwise actionable content under New Zealand law; or
           g. infringes individual privacy.
        3. You will not resell, redistribute, sub-license, or offer access to this
           data to any other person.
        4. You will report any error or omission you notice in the data to us.
        5. MBIE is the source of this data and may directly enforce these
           obligations against you.
        6. This data shows a business's legal registration status only. It does not
           confirm that a business is trading, hiring, or operating.
        """;
}
