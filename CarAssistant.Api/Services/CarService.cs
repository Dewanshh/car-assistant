using CarAssistant.Api.Entities;
using CarAssistant.Api.Repositories.Interfaces;

namespace CarAssistant.Api.Services;

public class CarService
{
    private readonly ICarRepository _carRepository;

    public CarService(ICarRepository carRepository)
    {
        _carRepository = carRepository;
    }

    public async Task<Guid> AddUserCarAsync(
        Guid userId,
        Guid carModelId,
        string? nickname,
        string? vin,
        string? registrationNumber)
    {
        var car = new Car
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CarModelId = carModelId,
            Nickname = nickname,
            Vin = vin,
            RegistrationNumber = registrationNumber
        };

        await _carRepository.AddAsync(car);
        return car.Id;
    }

    public Task<IEnumerable<Car>> GetUserCarsAsync(Guid userId)
        => _carRepository.GetByUserIdAsync(userId);
}
