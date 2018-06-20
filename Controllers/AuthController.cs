using System.Threading.Tasks;
using DatingApp.Api.Dtos;
using DatingApp.API.Data;
using DatingApp.API.Model;
using Microsoft.AspNetCore.Mvc;

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
       
    }
}