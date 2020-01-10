using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TestApp.API.Dtos;
using TestApp.API.Models;

namespace TestApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthController(
            IConfiguration config,
            IMapper mapper,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            // mapper creates new instance of User
            var userToCreate = _mapper.Map<User>(userForRegisterDto);
            
            // Identity method to add new user to db
            var result = await _userManager.CreateAsync(userToCreate, userForRegisterDto.Password);

            // every new added user has a "member" role
            _userManager.AddToRoleAsync(userToCreate, "Member").Wait();

            // if user is successfully added to db this method return 201 http response status 
            if (result.Succeeded)
            {
                return CreatedAtRoute("GetUser", new 
                { 
                    controller = "Users", 
                    id = userToCreate.Id 
                }, 
                _mapper.Map<UserForDetailedDto>(userToCreate));
            }

            // if user is not added method returns "badrequest"
            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            // Identity search for username in db
            var user = await _userManager.FindByNameAsync(userForLoginDto.UserName);

            // if there is no such user => return http response 400 
            if (user == null)
                return NotFound();

            // Identity confirms the password user entered
            var result = await _signInManager.CheckPasswordSignInAsync(user, userForLoginDto.Password, false);

            if (result.Succeeded)
            {
                var appUser = await _userManager.Users
                                        .FirstOrDefaultAsync(u => u.NormalizedUserName == userForLoginDto.UserName.ToUpper());
                var userToReturn = _mapper.Map<UserForListDto>(appUser);

                // if user exists and password is correct the method returns http response status 200 and users info
                // including new generated Jwt Bearer token
                return Ok(new
                {
                    token = GenerateJwtToken(appUser).Result,
                    user = userToReturn
                });
            }

            // if user is not found method returns http request status 400
            return Unauthorized();
        }

        // separate method for generation Jwt Bearer token for logged in user using name, id and role
        private async Task<string> GenerateJwtToken(User user)
        {
            // generating two claim types for users id and name
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            // Identity searches for roles current user has
            var roles = await _userManager.GetRolesAsync(user);

            // generating claims for users roles
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // generating key using secret string stored in settings.json file
            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

            // coding the key using Hmac512
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // generating token settings
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            // method returns token
            return tokenHandler.WriteToken(token);
        }
    }
}