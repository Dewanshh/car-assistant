using CarAssistant.Api.Entities;

namespace CarAssistant.Api.Repositories.Interfaces;

public interface IManualRepository
{
    Task AddAsync(ManualDocument manual);
    Task<IEnumerable<ManualDocument>> GetByCarModelIdAsync(Guid carModelId);
    Task<ManualDocument?> GetByIdAsync(Guid id);
}
