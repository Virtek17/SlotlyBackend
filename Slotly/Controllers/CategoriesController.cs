using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Slotly.Data;
using Slotly.Entities;
using Slotly.Interfaces;

namespace Slotly.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly SlotlyContext _context;

        public CategoriesController(SlotlyContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            // TODO: сделать через DTO
            var cateogories = await _context.Categories.ToListAsync();
            return Ok(cateogories);
        }
    }
}
