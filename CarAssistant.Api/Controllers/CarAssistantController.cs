using CarAssistant.Api.Dtos;
using CarAssistant.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarAssistant.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/cars/{carId:guid}/assistant")]
public class CarAssistantController : ControllerBase
{
    private readonly CarAssistantService _assistantService;

    public CarAssistantController(CarAssistantService assistantService)
    {
        _assistantService = assistantService;
    }

    [HttpPost("ask")]
    public async Task<IActionResult> Ask(Guid carId, [FromBody] CarAssistantRequest request)
    {
        var result = await _assistantService.AskAsync(carId, request.Question);

        return Ok(new CarAssistantResponse(
            result.Answer,
            result.RawDtcCodes,
            result.TelemetryTimestampUtc
        ));
    }
}
