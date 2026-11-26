using CarAssistant.Api.Entities;
using CarAssistant.Api.Repositories.Interfaces;

namespace CarAssistant.Api.Services;

public class CarModelService
{
    private readonly ICarModelRepository _carModelRepository;

    public CarModelService(ICarModelRepository carModelRepository)
    {
        _carModelRepository = carModelRepository;
    }

    public async Task<Guid> CreateAsync(string brand, string modelName, string variant, int yearFrom, int? yearTo)
    {
                Console.WriteLine("I Am herer");

        var model = new CarModel
        {
            Id = Guid.NewGuid(),
            Brand = brand,
            ModelName = modelName,
            Variant = variant,
            YearFrom = yearFrom,
            YearTo = yearTo
        };

        await _carModelRepository.AddAsync(model);
        return model.Id;
    }

    public Task<IEnumerable<CarModel>> GetAllAsync() => _carModelRepository.GetAllAsync();
}
