namespace CarAssistant.Api.Entities;

public class TelemetrySnapshot
{
    public Guid Id { get; set; }
    public Guid CarId { get; set; }
    public Guid ModelId {get;set;}
    public DateTime TimestampUtc { get; set; }

    public int Rpm { get; set; }
    public double SpeedKph { get; set; }
    public double CoolantTempC { get; set; }
    public double FuelLevelPercent { get; set; }

    // Comma-separated codes like "P0300,P0171"
    public string? ActiveDtcCodes { get; set; }
}
