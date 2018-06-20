using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.Api.Dtos;
using DatingApp.API.Data;
using DatingApp.API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class AuthController:Controller
    {
         private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository repo,IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }
     [HttpPost("register")]
     public async Task<IActionResult> Register([FromBody]UserRegisterDto userRegisterDto)
     {
         //validate user
         userRegisterDto.Username=userRegisterDto.Username.ToLower();
         if(await _repo.UserExists(userRegisterDto.Username))
         ModelState.AddModelError("UserName","Username is already taken");

         if(!ModelState.IsValid) return BadRequest(ModelState);

        
         var user =new User { UserName=userRegisterDto.Username};

         var createdUser = await _repo.Register(user,userRegisterDto.Password);
         return StatusCode(201);
     }
     [AllowAnonymous]
     [HttpPost("login")]
     public async Task<IActionResult> Login([FromBody]UserLoginDto userLoginDto)
     {
         var loggedInUser= await _repo.LoginAsync(userLoginDto.Username,userLoginDto.Password);
         if(loggedInUser==null) return Unauthorized();

         //generate token 

          var tokenHandler =new JwtSecurityTokenHandler();
          var key =Encoding.ASCII.GetBytes(_config.GetSection("AppSettings:Token").Value);
          var tokenDescription =new SecurityTokenDescriptor
           {
              Subject= new ClaimsIdentity(new Claim[]{
                  new Claim(ClaimTypes.NameIdentifier,loggedInUser.Id.ToString()),
                  new Claim(ClaimTypes.Name,loggedInUser.UserName)
              }),
             Expires=DateTime.Now.AddDays(1),
             SigningCredentials=new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha512Signature)

           };
         var token= tokenHandler.CreateToken(tokenDescription);
         var tokenString = tokenHandler.WriteToken(token);

         return Ok(new {tokenString});
     }
       
    }
}