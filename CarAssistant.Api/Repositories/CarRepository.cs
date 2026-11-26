using CarAssistant.Api.Data;
using CarAssistant.Api.Entities;
using CarAssistant.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarAssistant.Api.Repositories;

public class CarRepository : ICarRepository
{
    private readonly AppDbContext _db;

    public CarRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Car car)
    {
        await _db.Cars.AddAsync(car);
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<Car>> GetByUserIdAsync(Guid userId)
        => await _db.Cars.Where(c => c.UserId == userId).ToListAsync();
    public async Task<Car?> GetByIdAsync(Guid id)
{
    return await _db.Cars.FirstOrDefaultAsync(x => x.Id == id);
}
}
