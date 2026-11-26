using CarAssistant.Api.Entities;

namespace CarAssistant.Api.Repositories.Interfaces;

public interface ITelemetryRepository
{
    Task AddAsync(TelemetrySnapshot snapshot);
    Task<TelemetrySnapshot?> GetLatestForCarAsync(Guid carId);
}
