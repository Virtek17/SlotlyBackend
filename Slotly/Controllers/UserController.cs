using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Slotly.Entities;
using Slotly.Interfaces;
using Slotly.Models;
using System.Security.Cryptography;
using System.Text;

namespace Slotly.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IMapper _mapper;

        public UserController(
            IGenericRepository<User> userRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Проверка, что пользователь с таким email еще не существует
            var existingUsers = await _userRepository.GetAllAsync();
            if (existingUsers.Any(u => u.Email == createUserDto.Email))
            {
                return Conflict("User with this email already exists");
            }

            // Хеширование пароля
            var passwordHash = HashPassword(createUserDto.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                Email = createUserDto.Email,
                Phone = createUserDto.Phone,
                PasswordHash = passwordHash,
                TelegramId = createUserDto.TelegramId,
                Role = createUserDto.Role,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveAsync();

            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepository.GetAllAsync();
            return Ok(users);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var users = await _userRepository.GetAllAsync();
            var user = users.FirstOrDefault(u => u.Email == loginDto.Email);

            if (user == null)
            {
                return Unauthorized("Пользователь с таким Email еще не найден");
            }

            var password = HashPassword(loginDto.Password); 

            if (user.PasswordHash != password)
            {
                return Unauthorized("Неверный пароль");
            }

            var userDto = _mapper.Map<UserReadDto>(user);

            return Ok(userDto);
        }
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
