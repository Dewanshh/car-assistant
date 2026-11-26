using CarAssistant.Api.Data;
using CarAssistant.Api.Entities;
using CarAssistant.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarAssistant.Api.Repositories;

public class TelemetryRepository : ITelemetryRepository
{
    private readonly AppDbContext _db;

    public TelemetryRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(TelemetrySnapshot snapshot)
    {
        await _db.Telemetry.AddAsync(snapshot);
        await _db.SaveChangesAsync();
    }

    public async Task<TelemetrySnapshot?> GetLatestForCarAsync(Guid carId)
    {
        return await _db.Telemetry
            .Where(t => t.CarId == carId)
            .OrderByDescending(t => t.TimestampUtc)
            .FirstOrDefaultAsync();
    }
}
