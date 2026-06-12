using System.ComponentModel.DataAnnotations;
using CompanyVerification.Core;
using Microsoft.AspNetCore.Mvc;

namespace CompanyVerification.Api.Controllers;

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
        throw new NotImplementedException();
    }
}
