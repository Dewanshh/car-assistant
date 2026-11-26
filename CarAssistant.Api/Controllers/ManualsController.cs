using CarAssistant.Api.Dtos;
using CarAssistant.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarAssistant.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/car-models/{carModelId:guid}/manuals")]
public class ManualsController : ControllerBase
{
    private readonly ManualService _manualService;

    public ManualsController(ManualService manualService)
    {
        _manualService = manualService;
    }

    // POST api/car-models/{carModelId}/manuals
    [HttpPost]
    [RequestSizeLimit(50_000_000)] // 50 MB limit, adjust as needed
    public async Task<IActionResult> Upload(Guid carModelId, [FromForm] UploadManualRequest request)
    {
        if (request.File is null || request.File.Length == 0)
        {
            return BadRequest("File is required.");
        }

        using var ms = new MemoryStream();
        await request.File.CopyToAsync(ms);
        var bytes = ms.ToArray();

        var id = await _manualService.AddManualAsync(
            carModelId,
            request.Title,
            request.File.FileName,
            request.File.ContentType,
            bytes,
            request.ManualType
        );

        return Ok(new { id });
    }

    // GET api/car-models/{carModelId}/manuals
    [HttpGet]
    public async Task<IActionResult> GetList(Guid carModelId)
    {
        var manuals = await _manualService.GetForModelAsync(carModelId);

        var result = manuals.Select(m => new
        {
            m.Id,
            m.Title,
            m.ManualType,
            m.FileName,
            m.ContentType
        });

        return Ok(result);
    }

    // (Optional) GET api/car-models/{carModelId}/manuals/{manualId}/download
    [HttpGet("{manualId:guid}/download")]
    public async Task<IActionResult> Download(Guid carModelId, Guid manualId)
    {
        var manual = await _manualService.GetByIdAsync(manualId);
        if (manual == null || manual.CarModelId != carModelId)
            return NotFound();

        return File(manual.FileContent, manual.ContentType, manual.FileName);
    }
}
