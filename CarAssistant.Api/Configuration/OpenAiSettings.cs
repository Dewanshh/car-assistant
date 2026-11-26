namespace CarAssistant.Api.Configuration;

public class OpenAiSettings
{
    public string ApiKey { get; set; } = null!;
    public string Model { get; set; } = "gpt-4.1-mini";
}
