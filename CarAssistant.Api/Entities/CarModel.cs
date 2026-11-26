
namespace CarAssistant.Api.Entities;

public class CarModel
{
    public Guid Id { get; set; }
    public string Brand { get; set; } = null!;
    public string ModelName { get; set; } = null!;
    public string Variant { get; set; } = null!;
    public int YearFrom { get; set; }
    public int? YearTo { get; set; }
}
