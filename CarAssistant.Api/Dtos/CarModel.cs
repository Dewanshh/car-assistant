namespace CarAssistant.Api.Dtos;

public record CreateCarModelRequest(
    string Brand,
    string ModelName,
    string Variant,
    int YearFrom,
    int? YearTo);
