namespace CarAssistant.Api.Entities;

public class ManualDocument
{
    public Guid Id { get; set; }
    public Guid CarModelId { get; set; }

    public string Title { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = "application/pdf";

    // For now we keep PDF bytes in DB; later you can move to file storage / blob.
    public byte[] FileContent { get; set; } = null!;

    // Optional: tag to classify manuals
    public string? ManualType { get; set; } // e.g. "OwnerManual", "ServiceManual"
}
