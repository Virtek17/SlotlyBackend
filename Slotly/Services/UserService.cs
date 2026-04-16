using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Slotly.Data;
using Slotly.Entities;
using Slotly.Models.User;
using System.Security.Cryptography;
using System.Text;

namespace Slotly.Services
{
    public class UserService
    {
        private readonly SlotlyContext _context;
        private readonly IMapper _mapper;

        public UserService(
            SlotlyContext context,
            IMapper mapper
            )
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<UserReadDto?> AuthenticateAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>  u.Email == email);
            
            if (user == null || user.PasswordHash != HashPassword(password))
            {
                return null;
            }

            return _mapper.Map<UserReadDto>(user);
        }

        public async Task<UserReadDto> RegisterAsync(CreateUserDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                throw new Exception("Пользователь с таким email уже существует");

            var user = _mapper.Map<User>(dto);
            user.Id = Guid.NewGuid();
            user.PasswordHash = HashPassword(dto.Password);

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return _mapper.Map<UserReadDto>(user);
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
