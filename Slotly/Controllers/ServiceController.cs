using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Slotly.Entities;
using Slotly.Interfaces;

namespace Slotly.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IGenericRepository<Service> _serviceRepository;

        public ServiceController(IGenericRepository<Service> serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetServices()
        {
            var service = await _serviceRepository.GetAllAsync();

            return Ok(service);
        }
    }
}
