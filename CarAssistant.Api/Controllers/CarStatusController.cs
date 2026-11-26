using CarAssistant.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarAssistant.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/cars/{carId:guid}/status")]
public class CarStatusController : ControllerBase
{
    private readonly TelemetryIngestionService _telemetryService;
    private readonly CarHealthService _carHealthService;

    public CarStatusController(TelemetryIngestionService telemetryService, CarHealthService carHealthService)
    {
        _telemetryService = telemetryService;
        _carHealthService = carHealthService;
    }

    [HttpGet("health")]
    public async Task<IActionResult> GetHealth(Guid carId)
    {
        var snapshot = await _telemetryService.GetLatestAsync(carId);
        if (snapshot == null) return NotFound();

        var score = _carHealthService.CalculateHealthScore(snapshot);

        return Ok(new
        {
            healthScore = score,
            snapshot.TimestampUtc,
            snapshot.Rpm,
            snapshot.SpeedKph,
            snapshot.CoolantTempC,
            snapshot.FuelLevelPercent,
            snapshot.ActiveDtcCodes
        });
    }
}
