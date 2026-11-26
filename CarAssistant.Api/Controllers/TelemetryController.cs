using CarAssistant.Api.Dtos;
using CarAssistant.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarAssistant.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/cars/{carId:guid}/{modelId:guid}/telemetry")]
public class TelemetryController : ControllerBase
{
    private readonly TelemetryIngestionService _telemetryService;

    public TelemetryController(TelemetryIngestionService telemetryService)
    {
        _telemetryService = telemetryService;
    }

    // POST api/cars/{carId}/telemetry
    // This will be called by your OBD device / dummy simulator
    [HttpPost]
    public async Task<IActionResult> Ingest(Guid carId, Guid modelId, [FromBody] TelemetryIngestRequest request)
    {
        await _telemetryService.IngestAsync(
            carId,
            modelId,
            request.Rpm,
            request.SpeedKph,
            request.CoolantTempC,
            request.FuelLevelPercent,
            request.ActiveDtcCodes
        );

        return Ok();
    }

    // GET api/cars/{carId}/telemetry/latest
    // This will be called by frontend to show current car status
    [HttpGet("latest")]
    public async Task<IActionResult> GetLatest(Guid carId)
    {
        var snapshot = await _telemetryService.GetLatestAsync(carId);
        if (snapshot == null)
            return NotFound();

        var response = new TelemetryResponse(
            snapshot.TimestampUtc,
            snapshot.Rpm,
            snapshot.SpeedKph,
            snapshot.CoolantTempC,
            snapshot.FuelLevelPercent,
            snapshot.ActiveDtcCodes
        );

        return Ok(response);
    }
}
