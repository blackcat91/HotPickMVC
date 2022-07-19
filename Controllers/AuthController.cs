using HotPickMVC.Data;
using HotPickMVC.Data.Users;
using HotPickMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HotPickMVC.Controllers
{

    public class AuthController : Controller
    {
        private readonly IUserData _user;
        private readonly MongoDBAccess _db;

        public AuthController(IUserData user, MongoDBAccess db)
        {
            _user = user;
            _db = db;
        }
        [Route("login")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [Route("login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LogInModel logIn)
        {
            
               UserModel user =  await _user.LogIn(logIn);
                if(user.Error != string.Empty)
                {
                    ModelState.AddModelError("email", user.Error!);
                }
                else
                {
               
                    return RedirectToAction("Index", "Home");
                }
                
            
            
            return View();
        }

        [Route("register")]
        [HttpGet]

        public IActionResult Register()
        {
            return View();
        }

        [Route("register")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserModel user)
        {
        
                var newU =  await _user.CreateUser(user);
                if (newU.Error != string.Empty)
                {
                    ModelState.AddModelError("email", newU.Error!);
                }
                else
                {
                  return  RedirectToAction("Index");
                }
                
          
            
            
            return View();
        }

        [HttpGet]

        public async Task<IActionResult> LogOut()
        {
           
            Jwt? jwt = _db.VerifyJwt();
            if (jwt != null)
            {
                await _user.LogOut(jwt!.Id);
                return RedirectToAction("Index", "Auth");
            }
            return RedirectToAction("Index", "Home");
        }

    }
}
