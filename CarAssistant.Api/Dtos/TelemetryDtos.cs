namespace CarAssistant.Api.Dtos;

public record TelemetryIngestRequest(
    int Rpm,
    double SpeedKph,
    double CoolantTempC,
    double FuelLevelPercent,
    string? ActiveDtcCodes
);
