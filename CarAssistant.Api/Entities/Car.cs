namespace CarAssistant.Api.Entities;

public class Car
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CarModelId { get; set; }

    public string? Nickname { get; set; }
    public string? Vin { get; set; }
    public string? RegistrationNumber { get; set; }
}
