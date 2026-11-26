using CarAssistant.Api.Entities;
using CarAssistant.Api.Repositories.Interfaces;

namespace CarAssistant.Api.Services;

public class TelemetryIngestionService
{
    private readonly ITelemetryRepository _telemetryRepository;

    public TelemetryIngestionService(ITelemetryRepository telemetryRepository)
    {
        _telemetryRepository = telemetryRepository;
    }

    public async Task IngestAsync(Guid carId, Guid modelId,int rpm, double speedKph, double coolantTempC,
        double fuelLevelPercent, string? activeDtcCodes)
    {
        var snapshot = new TelemetrySnapshot
        {
            Id = Guid.NewGuid(),
            ModelId=modelId,
            CarId = carId,
            TimestampUtc = DateTime.UtcNow,
            Rpm = rpm,
            SpeedKph = speedKph,
            CoolantTempC = coolantTempC,
            FuelLevelPercent = fuelLevelPercent,
            ActiveDtcCodes = activeDtcCodes

        };

        await _telemetryRepository.AddAsync(snapshot);
    }

    public Task<TelemetrySnapshot?> GetLatestAsync(Guid carId)
        => _telemetryRepository.GetLatestForCarAsync(carId);
}
