using CarAssistant.Api.Dtos;
using CarAssistant.Api.Extensions;
using CarAssistant.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarAssistant.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CarsController : ControllerBase
{
    private readonly CarService _carService;

    public CarsController(CarService carService)
    {
        _carService = carService;
    }

    [HttpPost]
    public async Task<IActionResult> Add(AddCarRequest request)
    {
        var userId = User.GetUserId();
        var id = await _carService.AddUserCarAsync(
            userId,
            request.CarModelId,
            request.Nickname,
            request.Vin,
            request.RegistrationNumber);

        return Ok(new { id });
    }

    [HttpGet("my")]
    public async Task<IActionResult> MyCars()
    {
        var userId = User.GetUserId();
        var cars = await _carService.GetUserCarsAsync(userId);
        return Ok(cars);
    }
}
