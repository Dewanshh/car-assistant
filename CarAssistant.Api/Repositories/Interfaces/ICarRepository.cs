using CarAssistant.Api.Entities;

namespace CarAssistant.Api.Repositories.Interfaces;

public interface ICarRepository
{
    Task AddAsync(Car car);
    Task<IEnumerable<Car>> GetByUserIdAsync(Guid userId);
    Task<Car?> GetByIdAsync(Guid id);   // ✅ ADD THIS

}
