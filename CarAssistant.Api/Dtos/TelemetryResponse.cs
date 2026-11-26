public record TelemetryResponse(
    DateTime TimestampUtc,
    int Rpm,
    double SpeedKph,
    double CoolantTempC,
    double FuelLevelPercent,
    string? ActiveDtcCodes
);
