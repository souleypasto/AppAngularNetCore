using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using API.DTOs;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {

        private readonly ITokenService _tokenService;
        public AccountController(DataContext context, ITokenService tokenService):base(context)
        {
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto) {
            
            
            if (await UserExixts (registerDto.UserName))
                return BadRequest("Username already exists ");

            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = registerDto.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto 
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto){
            var user = await _context.Users
                .SingleOrDefaultAsync(x => x.UserName == loginDto.UserName);

            if (user == null)
                return Unauthorized("Invalid userName");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var ComputedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < ComputedHash.Length; i++) {
                if(ComputedHash[i] != user.PasswordHash[i]) 
                    return Unauthorized("Invalid Password ");
            }

            return new UserDto 
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        private async Task<bool> UserExixts(string username){
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}