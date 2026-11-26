using System.Text;
using CarAssistant.Api.Configuration;
using CarAssistant.Api.Entities;
using CarAssistant.Api.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using UglyToad.PdfPig;

namespace CarAssistant.Api.Services;

public class CarAssistantService
{
    private readonly ICarRepository _carRepository;
    private readonly ICarModelRepository _carModelRepository;
    private readonly ITelemetryRepository _telemetryRepository;
    private readonly ManualService _manualService;
    private readonly OpenAIClient _openAiClient;
    private readonly OpenAiSettings _settings;

    public CarAssistantService(
        ICarRepository carRepository,
        ICarModelRepository carModelRepository,
        ITelemetryRepository telemetryRepository,
        ManualService manualService,
        OpenAIClient openAiClient,
        IOptions<OpenAiSettings> openAiOptions)
    {
        _carRepository = carRepository;
        _carModelRepository = carModelRepository;
        _telemetryRepository = telemetryRepository;
        _manualService = manualService;
        _openAiClient = openAiClient;
        _settings = openAiOptions.Value;
    }

    public async Task<CarAssistantResult> AskAsync(Guid carId, string question)
    {
        var car = await _carRepository.GetByIdAsync(carId)
                  ?? throw new Exception("Car not found");

        var model = await _carModelRepository.GetByIdAsync(car.CarModelId)
                    ?? throw new Exception("Car model not found");

        var telemetry = await _telemetryRepository.GetLatestForCarAsync(carId);

        var manuals = await _manualService.GetForModelAsync(model.Id);
        var manualContext = BuildManualContext(manuals);

        var prompt = BuildPrompt(car, model, telemetry, manualContext, question);

        var answer = await CallOpenAiAsync(prompt);

        return new CarAssistantResult(
            answer,
            telemetry?.ActiveDtcCodes,
            telemetry?.TimestampUtc
        );
    }

    private string BuildManualContext(IEnumerable<ManualDocument> manuals)
    {
        if (!manuals.Any()) return "No manuals uploaded.";

        var sb = new StringBuilder();

        foreach (var m in manuals)
        {
            sb.AppendLine($"### {m.Title}");
            sb.AppendLine(ExtractTextFromPdf(m.FileContent));
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private string ExtractTextFromPdf(byte[] bytes)
    {
        try
        {
            using var ms = new MemoryStream(bytes);
            using var doc = PdfDocument.Open(ms);

            var sb = new StringBuilder();
            int pages = Math.Min(3, doc.NumberOfPages);

            for (int i = 1; i <= pages; i++)
                sb.AppendLine(doc.GetPage(i).Text);

            return sb.Length > 4000
                ? sb.ToString()[..4000]
                : sb.ToString();
        }
        catch
        {
            return "Failed to extract PDF text.";
        }
    }

    private static string BuildPrompt(
        Car car,
        CarModel model,
        TelemetrySnapshot? telemetry,
        string manualContext,
        string question)
    {
        var sb = new StringBuilder();

        sb.AppendLine("You are a professional car diagnostic AI.");
        sb.AppendLine();

        sb.AppendLine($"Car: {model.Brand} {model.ModelName} {model.Variant}");
        sb.AppendLine($"Registration: {car.RegistrationNumber}");
        sb.AppendLine();

        if (telemetry != null)
        {
            sb.AppendLine("Latest Telemetry:");
            sb.AppendLine($"RPM: {telemetry.Rpm}");
            sb.AppendLine($"Speed: {telemetry.SpeedKph}");
            sb.AppendLine($"Coolant: {telemetry.CoolantTempC}");
            sb.AppendLine($"Fuel: {telemetry.FuelLevelPercent}");
            sb.AppendLine($"DTC: {telemetry.ActiveDtcCodes}");
            sb.AppendLine();
        }

        sb.AppendLine("Manual Context:");
        sb.AppendLine(manualContext);
        sb.AppendLine();

        sb.AppendLine("User Question:");
        sb.AppendLine(question);
        sb.AppendLine();

        sb.AppendLine("Provide:");
        sb.AppendLine("1. Health Summary");
        sb.AppendLine("2. DTC explanation");
        sb.AppendLine("3. Risk level");
        sb.AppendLine("4. DIY vs Service Advice");
        sb.AppendLine("5. Safety warning");

        return sb.ToString();
    }

private async Task<string> CallOpenAiAsync(string prompt)
{
    var chatClient = _openAiClient.GetChatClient(_settings.Model);

    var messages = new List<ChatMessage>
    {
        new SystemChatMessage("You are an expert automotive engineer."),
        new UserChatMessage(prompt)
    };

    var result = await chatClient.CompleteChatAsync(messages);

    if (result?.Value?.Content == null || result.Value.Content.Count == 0)
        return "No response from AI.";

    var sb = new StringBuilder();

    foreach (var part in result.Value.Content)
    {
        // ✅ v2.7.0 safe extraction
        var textProp = part.GetType().GetProperty("Text");
        if (textProp != null)
        {
            var value = textProp.GetValue(part)?.ToString();
            if (!string.IsNullOrWhiteSpace(value))
                sb.Append(value);
        }
        else
        {
            // fallback safety
            sb.Append(part.ToString());
        }
    }

    return sb.Length == 0 ? "No response from AI." : sb.ToString();
}


}

public record CarAssistantResult(
    string Answer,
    string? RawDtcCodes,
    DateTime? TelemetryTimestampUtc
);
