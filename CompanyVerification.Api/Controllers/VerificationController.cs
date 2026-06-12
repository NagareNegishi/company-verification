using System.ComponentModel.DataAnnotations;
using CompanyVerification.Core;
using Microsoft.AspNetCore.Mvc;

namespace CompanyVerification.Api.Controllers;

/// <summary>
/// Exposes company verification searches over HTTP.
/// Delegates to the <see cref="IVerificationProvider"/> registered for the requested country.
/// </summary>
[ApiController]
[Route("verify")]
public sealed class VerificationController : ControllerBase
{
    private readonly IEnumerable<IVerificationProvider> _providers;

    public VerificationController(IEnumerable<IVerificationProvider> providers)
    {
        _providers = providers;
    }

    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery][Required] string name,
        [FromQuery][Required] string country,
        CancellationToken cancellationToken)
    {
        var provider = _providers.FirstOrDefault(
            p => p.SupportedCountries.Contains(country.ToUpperInvariant()));

        if (provider is null)
            return NotFound();

        try
        {
            var results = await provider.Search(name, country, cancellationToken);
            return Ok(results); // empty list is a valid result, not a 404
        }
        catch (ArgumentException ex)
        {
            // VerificationProviderBase validates name/country format before hitting the registry
            return BadRequest(ex.Message);
        }
    }
}
