namespace CarAssistant.Api.Dtos;

public record CarAssistantRequest(string Question);

public record CarAssistantResponse(
    string Answer,
    string? RawDtcCodes,
    DateTime? TelemetryTimestampUtc
);
