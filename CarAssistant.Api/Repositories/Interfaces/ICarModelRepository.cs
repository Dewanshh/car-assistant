using CarAssistant.Api.Entities;

namespace CarAssistant.Api.Repositories.Interfaces;

public interface ICarModelRepository
{
    Task<CarModel?> GetByIdAsync(Guid id);
    Task<IEnumerable<CarModel>> GetAllAsync();
    Task AddAsync(CarModel model);
}
