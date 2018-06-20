using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.Api.Dtos;
using DatingApp.API.Data;
using DatingApp.API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.Api.Controllers
{
    [Route("api/[controller]")]
    public class AuthController:Controller
    {
         private readonly IAuthRepository _repo;
        public AuthController(IAuthRepository repo)
        {
            _repo = repo;
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
     [HttpPost("login")]
     public async Task<IActionResult> Login([FromBody]UserLoginDto userLoginDto)
     {
         var loggedInUser= await _repo.LoginAsync(userLoginDto.Username,userLoginDto.Password);
         if(loggedInUser==null) return Unauthorized();

         //generate token 

          var tokenHandler =new JwtSecurityTokenHandler();
          var key =Encoding.ASCII.GetBytes(" Super Secret Key");
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