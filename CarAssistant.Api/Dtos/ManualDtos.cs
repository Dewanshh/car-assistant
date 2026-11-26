namespace CarAssistant.Api.Dtos;

public class UploadManualRequest
{
    public string Title { get; set; } = null!;
    public string? ManualType { get; set; }
    public IFormFile File { get; set; } = null!;
}
