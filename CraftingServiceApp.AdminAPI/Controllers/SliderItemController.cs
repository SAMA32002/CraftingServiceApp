using CraftingServiceApp.AdminAPI.Dtos;
using CraftingServiceApp.AdminAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class SliderItemController : ControllerBase
{
    private readonly ISliderItemService _sliderItemService;

    public SliderItemController(ISliderItemService sliderItemService)
    {
        _sliderItemService = sliderItemService;
    }

    [HttpPost("CreateSliderItem")]
    public async Task<IActionResult> CreateSliderItem([FromForm] SliderItemDTO sliderItemDTO)
    {
        var sliderItem = await _sliderItemService.CreateSliderItemAsync(sliderItemDTO);
        return Ok(sliderItem);
    }

    [HttpPut("UpdateSliderItem/{id}")]
    public async Task<IActionResult> UpdateSliderItem(int id, [FromForm] SliderItemDTO sliderItemDTO)
    {
        var sliderItem = await _sliderItemService.UpdateSliderItemAsync(id, sliderItemDTO);

        if (sliderItem == null)
        {
            return NotFound();
        }

        return Ok(sliderItem);
    }

    [HttpDelete("DeleteSliderItem/{id}")]
    public async Task<IActionResult> DeleteSliderItem(int id)
    {
        var result = await _sliderItemService.DeleteSliderItemAsync(id);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("GetAllSliderItems")]
    public async Task<IActionResult> GetAllSliderItems()
    {
        var sliderItems = await _sliderItemService.GetAllSliderItemsAsync();
        return Ok(sliderItems);
    }
}
