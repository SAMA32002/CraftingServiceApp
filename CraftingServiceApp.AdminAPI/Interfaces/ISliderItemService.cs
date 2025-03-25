using CraftingServiceApp.AdminAPI.Dtos;
using CraftingServiceApp.Domain.Entities;

namespace CraftingServiceApp.AdminAPI.Interfaces
{
    public interface ISliderItemService
    {
        Task<SliderItem> CreateSliderItemAsync(SliderItemDTO sliderItemDTO);
        Task<SliderItem> UpdateSliderItemAsync(int id, SliderItemDTO sliderItemDTO);
        Task<bool> DeleteSliderItemAsync(int id);
        Task<IEnumerable<SliderItem>> GetAllSliderItemsAsync();
    }
}
