using conj_ai.Controllers.Conjugation.Handlers;
using conj_ai.Models.Conjugation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace conj_ai.Controllers.Conjugation;

[EnableRateLimiting("ip")]
[ApiController]
[Route("api/[controller]")]

public class ConjugationController(IMediator mediator) : ControllerBase
{
    [HttpGet("fr/{term}")]
    public async Task<IActionResult> Get(string term, CancellationToken ct)
    {
        return Ok(await mediator.Send(new ConjugationQuery<FrenchConjugation>(term), ct));
    }
}