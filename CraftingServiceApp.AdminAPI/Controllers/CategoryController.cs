using CraftingServiceApp.AdminAPI.Dtos;
using CraftingServiceApp.Application.Interfaces;
using CraftingServiceApp.Domain.Entities;
using CraftingServiceApp.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CraftingServiceApp.AdminAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IRepository<Category> _categoryRepository;
        private readonly ApplicationDbContext _context;

        public CategoryController(IRepository<Category> categoryRepository, ApplicationDbContext context)
        {
            _categoryRepository = categoryRepository;
            _context = context;
        }

        [HttpDelete("DeleteCategory/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound(new { Message = "Category not found" });
            }

            var services = await _context.Services
                .Where(s => s.CategoryId == id)
                .ToListAsync();

            if (services.Any())
            {
                return BadRequest(new { Message = "Cannot delete category, services are still associated with it" });
            }

            _categoryRepository.Delete(category);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Category deleted successfully" });
        }


        [HttpPost]
        public async Task<IActionResult> AddCategory(CategoryDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new { Message = "Invalid category data" });
            }

            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description,

            };


            await _categoryRepository.AddAsync(category);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCategory", new { id = category.Id }, new { Message = "Category added successfully", Category = category });
        }

        [HttpGet("GetCategory/{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound(new { Message = "Category not found" });
            }

            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };

            return Ok(categoryDto);
        }


        [HttpPut("EditCategory/{id}")]
        public async Task<IActionResult> EditCategory(int id, CategoryDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new { Message = "Invalid category data" });
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound(new { Message = "Category not found" });
            }

            category.Name = dto.Name;
            category.Description = dto.Description;

            await _context.SaveChangesAsync();

            var updatedCategoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };

            return Ok(new { Message = "Category updated successfully", Category = updatedCategoryDto });
        }


        [HttpGet("GetAllCategories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _context.Categories.ToListAsync();

            var categoryDtos = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            }).ToList();

            return Ok(categoryDtos);
        }

    }

}
