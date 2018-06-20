using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Model;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Api.Controllers
{
    [Route("api/[controller]")]
    public class RegisterController:Controller
    {
         private readonly IAuthRepository _repo;
        public RegisterController(IAuthRepository repo)
        {
            _repo = repo;
        }
     [HttpPost("register")]
     public async Task<IActionResult> Register(string username,string password)
     {
         //validate user

         username=username.ToLower();
         var user =new User { UserName=username};

         var createdUser = await _repo.Register(user,password);
         return StatusCode(201);
     }
       
    }
}