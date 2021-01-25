using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using JogoApi.Services;
using JogoApi.Configs;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using AutoMapper;
using Microsoft.Extensions.Options;
using System.Linq;
using JogoApi.Model.Users;
using JogoApi.Models;

namespace JogoApi.Controllers
{
    ///User and admin has access
    [Route("api/TowerResistance")]
    [ApiController]
  //  [Authorize(Roles = "admin,user")]

    public class UsersController : ControllerBase
    {

        private readonly UserService _userService;
        private readonly IMapper _mapper;

        private readonly AppSettings _appSettings;

        public UsersController(UserService userService, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        /// <summary>
        /// List of all players
        /// </summary>
        [HttpGet("Users")]
        [Authorize(Roles = "user,admin")]
        public IActionResult GetAll()
        {

            var user = _userService.GetAll();
            var userModel = _mapper.Map<List<UserModel>>(user.Where(x => x.Role == "user"));
            return Ok(userModel);
        }

        /// <summary>
        /// Get user information
        /// </summary>
        /// <param name="username"></param>
        [Authorize(Roles = "user,admin")]
        [HttpGet("Users/{username}", Name = "Get User")]
        public IActionResult GetByUsername(string username)
        {
 
            var user = _userService.GetAll().SingleOrDefault(u => u.Username==username);
            if (user == null)
            {
                return NotFound();
            }
            var userModel = _mapper.Map<UserModel>(user);
            return Ok(userModel);
        }

        /// <summary>
        /// Edit user 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateModel"></param>
        [Authorize(Roles = "user,admin")]
        [HttpPut("Users/Edit/{id}")]
        public IActionResult Update(int id,
                                    [FromBody] UpdateModel updateModel)
        {
            var oldUser = _userService.GetById(id);
            if (oldUser == null)
            {
                return NotFound();
            }
            else if (updateModel.Role=="admin")
            {
                return BadRequest();
            }

            var user = _mapper.Map<User>(updateModel);
            //user.Username = username;
            try
            {
                _userService.Update(user, updateModel.Password);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        /// <summary>
        /// User sign in
        /// </summary>
        [AllowAnonymous]
        [HttpPost("Users/Signin")]
        public IActionResult Create([FromBody] RegisterModel registerModel)
        {
            var user = _mapper.Map<User>(registerModel);
            try
            {
                user.Activated = true;
                user.Role = "user";
                _userService.Create(user, registerModel.Password);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// User login
        /// </summary>
        [AllowAnonymous]
        [HttpPost("Users/Login")]

        public IActionResult Authenticate([FromBody] AuthenticateModel authModel)
        {

            var user = _userService.Authenticate(authModel.Username, authModel.Password);
            if (user == null)
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]{
                new Claim(ClaimTypes.Name, user.Username.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            }),
                Expires = DateTime.UtcNow.AddHours(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                Id= user.Id,
                Username = user.Username,
                Role = user.Role,
                TokenContext = tokenString
            });
    }


        // [HttpPost]
        // [Route("Logout")]
        // public IActionResult Logout()
        // {           // Well, What do you want to do here ?
        //             // Wait for token to get expired OR 
        //             // Maintain token cache and invalidate the tokens after logout method is called	
        //     // return Ok(new { Token = token, Message = "Logged Out" });
        // }



    }
}
