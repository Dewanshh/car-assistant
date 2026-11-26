using CarAssistant.Api.Data;
using CarAssistant.Api.Entities;
using CarAssistant.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarAssistant.Api.Repositories;

public class ManualRepository : IManualRepository
{
    private readonly AppDbContext _db;

    public ManualRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(ManualDocument manual)
    {
        await _db.Manuals.AddAsync(manual);
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<ManualDocument>> GetByCarModelIdAsync(Guid carModelId)
    {
        return await _db.Manuals
            .Where(m => m.CarModelId == carModelId)
            .ToListAsync();
    }

    public async Task<ManualDocument?> GetByIdAsync(Guid id)
    {
        return await _db.Manuals.FindAsync(id);
    }
}
