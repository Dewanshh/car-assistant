using CarAssistant.Api.Dtos;
using CarAssistant.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarAssistant.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CarModelsController : ControllerBase
{
    private readonly CarModelService _carModelService;

    public CarModelsController(CarModelService carModelService)
    {
        _carModelService = carModelService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCarModelRequest request)
    {
        Console.WriteLine("I Am herer");
        var id = await _carModelService.CreateAsync(
            request.Brand,
            request.ModelName,
            request.Variant,
            request.YearFrom,
            request.YearTo);
        
        return Ok(new { id });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var models = await _carModelService.GetAllAsync();
        return Ok(models);
    }
}
