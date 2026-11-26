using CarAssistant.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarAssistant.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<CarModel> CarModels => Set<CarModel>();
    public DbSet<Car> Cars => Set<Car>();
    public DbSet<ManualDocument> Manuals => Set<ManualDocument>(); // 👈 add this
    public DbSet<TelemetrySnapshot> Telemetry => Set<TelemetrySnapshot>();
}
