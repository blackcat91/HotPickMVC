using HotPickMVC.Data;
using HotPickMVC.Data.Portfolios;
using HotPickMVC.Data.Users;
using HotPickMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotPickMVC.Controllers
{
    public class UserController : Controller
    {
        private readonly IPortfolioData _port;
        private readonly MongoDBAccess _db;
        private readonly IUserData _user;

        public UserController(IPortfolioData port, MongoDBAccess db, IUserData user)
        {
            _port = port;
            _db = db;
            _user = user;
        }


        public IActionResult Index()
        {
            return RedirectToAction("Dashboard", "User");
        }

        public async Task<IActionResult> Dashboard()
        {
            Jwt? jwt = _db.VerifyJwt();

            if (jwt != null)
            {
                ViewData["Jwt"] = jwt.Token;
                var user = await _user.GetUser(jwt.Id);
                var ports = await _port.getPortfolios(jwt.Id);
                UserPageModel pageData = new UserPageModel();
                pageData.User = user;
                pageData.Portfolios = ports;
                return View(pageData);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Dashboard(UserPageModel userData)
        {
            Jwt? jwt = _db.VerifyJwt();

            if (jwt != null)
            {
                ViewData["Jwt"] = jwt.Token;
                try
                {
                    var user = await _user.UpdateUser(userData.User!);

                    var ports = await _port.getPortfolios(jwt.Id);
                    UserPageModel pageData = new UserPageModel();
                    pageData.User = user;
                    pageData.Portfolios = ports;
                    return View(pageData);
                }
                catch(Exception e)
                {
                    ModelState.AddModelError("username", e.Message);
                    return View(userData);
                }
                
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> ChangePassword()
        {
            Jwt? jwt = _db.VerifyJwt();

            if (jwt != null)
            {
                ViewData["Jwt"] = jwt.Token;
                var pwd = new ChangePasswordModel();
                pwd.Id = jwt.Id;
                return View(pwd);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel pwd)
        {
            Jwt? jwt = _db.VerifyJwt();

            if (jwt != null)
            {
                ViewData["Jwt"] = jwt.Token;
                pwd.Id = jwt.Id;

                var change = await _user.ChangePassword(pwd);

                if (change is int)
                {
                    return RedirectToAction("Dashboard", "User");
                }
                ModelState.AddModelError("currentpassword", change.Error);
                return View(pwd);



            }

            return RedirectToAction("Index", "Home");
        }
    }
}
