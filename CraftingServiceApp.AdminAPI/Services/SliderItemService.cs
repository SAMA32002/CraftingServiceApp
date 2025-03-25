using CraftingServiceApp.AdminAPI.Dtos;
using CraftingServiceApp.AdminAPI.Interfaces;
using CraftingServiceApp.Domain.Entities;
using CraftingServiceApp.Infrastructure.Data;

namespace CraftingServiceApp.AdminAPI.Services
{
    public class SliderItemService : ISliderItemService
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;

        public SliderItemService(ApplicationDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        public async Task<SliderItem> CreateSliderItemAsync(SliderItemDTO sliderItemDTO)
        {
            // Upload the file (image/video)
            var filePath = await _fileService.UploadFileAsync(sliderItemDTO.MediaFile, sliderItemDTO.FileType);

            var sliderItem = new SliderItem
            {
                FilePath = filePath,
                FileType = sliderItemDTO.FileType,
                IsActive = true,  // By default, set the item as active
                Description = sliderItemDTO.Description
            };

            _context.SliderItems.Add(sliderItem);
            await _context.SaveChangesAsync();

            return sliderItem;
        }

        public async Task<SliderItem> UpdateSliderItemAsync(int id, SliderItemDTO sliderItemDTO)
        {
            var sliderItem = await _context.SliderItems.FindAsync(id);

            if (sliderItem == null)
            {
                return null;
            }

            // If a new file is provided, delete the old one and upload the new one
            if (sliderItemDTO.MediaFile != null)
            {
                await _fileService.DeleteFileAsync(sliderItem.FilePath); // Delete old file
                sliderItem.FilePath = await _fileService.UploadFileAsync(sliderItemDTO.MediaFile, sliderItemDTO.FileType); // Upload new file
            }

            sliderItem.FileType = sliderItemDTO.FileType;
            sliderItem.Description = sliderItemDTO.Description;

            _context.SliderItems.Update(sliderItem);
            await _context.SaveChangesAsync();

            return sliderItem;
        }

        public async Task<bool> DeleteSliderItemAsync(int id)
        {
            var sliderItem = await _context.SliderItems.FindAsync(id);

            if (sliderItem == null)
            {
                return false;
            }

            // Delete the file from the server
            await _fileService.DeleteFileAsync(sliderItem.FilePath);

            _context.SliderItems.Remove(sliderItem);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<SliderItem>> GetAllSliderItemsAsync()
        {
            return  _context.SliderItems.ToList();
        }
    }

}
