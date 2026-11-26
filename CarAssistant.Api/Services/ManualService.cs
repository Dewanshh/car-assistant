using CarAssistant.Api.Entities;
using CarAssistant.Api.Repositories.Interfaces;

namespace CarAssistant.Api.Services;

public class ManualService
{
    private readonly IManualRepository _manualRepository;

    public ManualService(IManualRepository manualRepository)
    {
        _manualRepository = manualRepository;
    }

    public async Task<Guid> AddManualAsync(
        Guid carModelId,
        string title,
        string fileName,
        string contentType,
        byte[] fileContent,
        string? manualType)
    {
        var manual = new ManualDocument
        {
            Id = Guid.NewGuid(),
            CarModelId = carModelId,
            Title = title,
            FileName = fileName,
            ContentType = contentType,
            FileContent = fileContent,
            ManualType = manualType
        };

        await _manualRepository.AddAsync(manual);
        return manual.Id;
    }

    public Task<IEnumerable<ManualDocument>> GetForModelAsync(Guid carModelId)
        => _manualRepository.GetByCarModelIdAsync(carModelId);

    public Task<ManualDocument?> GetByIdAsync(Guid id)
        => _manualRepository.GetByIdAsync(id);
}
