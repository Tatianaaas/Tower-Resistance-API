using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using JogoApi.Services;
using AutoMapper;
using Microsoft.Extensions.Options;
using JogoApi.Configs;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using JogoApi.Model.Users;
using JogoApi.Models;

namespace JogoApi.Controllers
{

    /// <summary>
    /// Only admin has access
    /// </summary>
    [Route("api/TowerResistance")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;

        public AdminController(UserService userService, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }


        /// <summary>
        ///List of all admin 
        /// </summary>
        [Authorize(Roles = "admin")]
        [HttpGet("Admin/List")]
        public IActionResult GetAll()
        {

            var user = _userService.GetAll();
           // var userModel = _mapper.Map<List<UserModel>>(user.Where(x => x.Role == "user"));
            return Ok(user.ToList());

        }

        /// <summary>
        /// Get information of a player
        /// </summary>
        ///<param name="id"></param>
        [Authorize(Roles = "admin")]
        [HttpGet("Admin/{id}")]
        public IActionResult GetByUsername(int id)
        {

            var user = _userService.GetById(id);
            if (user == null)
            {
                return NotFound();
            }
            var userModel = _mapper.Map<UserModel>(user);
            return Ok(userModel);
        }

        ///<summary>
        ///Change the role of the admin to a simple player
        /// </summary>
        ///<param name="id"></param>
          ///<param name="updateModel"></param>
        [Authorize(Roles = "admin")]
        [HttpPut("Admin/Delete/{id}")]
        public IActionResult UpdateRole(int id, [FromBody] UpdateModel updateModel)
        {
            var oldUser = _userService.GetById(id);
            if (oldUser == null)
            {
                return NotFound();
            }

            var user = _mapper.Map<User>(updateModel);
            user.Role = "user";
            try
            {
                _userService.Update(user, updateModel.Role);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }


        /// <summary>
        /// Edit admin
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateModel"></param>
        [Authorize(Roles = "admin")]
        [HttpPut("Admin/Edit/{id}")]
        public IActionResult Update(int id,
                                    [FromBody] UpdateModel updateModel)
        {
            var oldUser = _userService.GetById(id);
            if (oldUser == null)
            {
                return NotFound();
            }
            else if (oldUser.Role == "user")
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

        ///<summary>
        ///Admin registration
        ///</summary>
        [AllowAnonymous]
        [HttpPost("Admin/Signin")]
        public IActionResult Create([FromBody] RegisterModel registerModel)
        {
            var user = _mapper.Map<User>(registerModel);
            try
            {
                user.Role = "admin";
                _userService.Create(user, registerModel.Password);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        ///<summary>
        ///Admin login
        ///</summary>
        [AllowAnonymous]
        [HttpPost("Admin/Login")]
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
                Role= user.Role,
                TokenContext = tokenString
            });
        }


    }
}
