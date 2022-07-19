using HotPickMVC.Data;
using HotPickMVC.Data.Portfolios;
using HotPickMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotPickMVC.Controllers
{
    public class PortfolioController : Controller
    {
        private readonly IPortfolioData _port;
        private readonly MongoDBAccess _db;

        public PortfolioController(IPortfolioData port, MongoDBAccess db)
        {
            _port = port;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            Jwt? jwt = _db.VerifyJwt();

            if (jwt != null)
            {
                ViewData["Jwt"] = jwt.Token;
                var ports = await _port.getPortfolios(jwt.Id);
                return View(ports);
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Create()
        {
            Jwt? jwt = _db.VerifyJwt();

            if (jwt != null)
            {
                ViewData["Jwt"] = jwt.Token;
                return View();
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(PortfolioModel port)
        {

            Jwt? jwt = _db.VerifyJwt();
            port.UserId = jwt.Id;

            if (jwt != null)
            {
                ViewData["Jwt"] = jwt.Token;


                try
                {
                    await _port.CreatePortfolio(port);
                    return RedirectToAction("Index", "Portfolio");
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError("name","Error Creating Portfolio");
                    return View();
                }
               
            }
            
            return RedirectToAction("Index", "Home");

        }

        public async Task<IActionResult> Edit(string portId)
        {
            Jwt? jwt = _db.VerifyJwt();

            if (jwt != null)
            {
                ViewData["Jwt"] = jwt.Token;
                var ports = await _port.getPortfolios(jwt.Id);
                var portfolio = ports.Find(p => p.Id == portId);
                return View(portfolio);
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PortfolioModel portfolio)
        {
            Jwt? jwt = _db.VerifyJwt();

            if (jwt != null)
            {
                ViewData["Jwt"] = jwt.Token;
                await _port.EditPortfolio(portfolio);
                var ports = await _port.getPortfolios(jwt.Id);
                var port = ports.Find(p => p.Id == portfolio.Id);
                return View(port);
            }

            return RedirectToAction("Index", "Home");
        }


    }
}
