namespace CarAssistant.Api.Dtos;

public record AddCarRequest(
    Guid CarModelId,
    string? Nickname,
    string? Vin,
    string? RegistrationNumber);
