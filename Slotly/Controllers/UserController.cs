using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Slotly.Data;
using Slotly.Entities;
using Slotly.Interfaces;
using Slotly.Models.Auth;
using Slotly.Models.User;
using Slotly.Services;
using System.Security.Cryptography;
using System.Text;

namespace Slotly.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly SlotlyContext _context;
        private readonly UserService _userService;
        private readonly IMapper _mapper;

        public UserController(
            SlotlyContext context,
            UserService userService,
            IMapper mapper)
        {
            _context = context;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            try
            {
                var result = await _userService.RegisterAsync(dto);
                return CreatedAtAction(nameof(GetUserById), new {id = result.Id}, result);
            } catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = _mapper.Map<UserReadDto>(user);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            var result = _mapper.Map<List<UserReadDto>>(users);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {

            var user = await _userService.AuthenticateAsync(dto.Email, dto.Password); 

            if (user == null)
            {
                return Unauthorized("Invalid credentials");
            }

            return Ok(user);    


        }
       
    }
}
