using CarAssistant.Api.Data;
using CarAssistant.Api.Entities;
using CarAssistant.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace CarAssistant.Api.Repositories;

public class CarModelRepository : ICarModelRepository
{
    private readonly AppDbContext _db;

    public CarModelRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<CarModel?> GetByIdAsync(Guid id)
        => await _db.CarModels.FindAsync(id);

    public async Task<IEnumerable<CarModel>> GetAllAsync()
        => await _db.CarModels.ToListAsync();

    public async Task AddAsync(CarModel model)
    {
        await _db.CarModels.AddAsync(model);
        await _db.SaveChangesAsync();
    }
}
