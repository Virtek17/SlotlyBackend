using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Slotly.Entities;
using Slotly.Interfaces;

namespace Slotly.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IGenericRepository<Category> _categoryRepository;

        public CategoriesController(IGenericRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var cateogories = await _categoryRepository.GetAllAsync();

            return Ok(cateogories);
        }
    }
}
