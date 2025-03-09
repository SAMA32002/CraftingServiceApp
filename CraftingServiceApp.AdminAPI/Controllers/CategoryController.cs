using CraftingServiceApp.Application.Interfaces;
using CraftingServiceApp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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
    }
}
