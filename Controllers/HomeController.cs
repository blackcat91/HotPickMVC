using HotPickMVC.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System;
using HotPickMVC.Data;
using HotPickMVC.Data.Stocks;

namespace HotPickMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MongoDBAccess _db;
        private readonly IStockData _stocks;

        public HomeController(ILogger<HomeController> logger, MongoDBAccess db, IStockData stocks)
        {
            _logger = logger;
            _db = db;
            _stocks = stocks;
        }

        public async Task<IActionResult> Index()
        {
            List<StockModel> topTen = await _stocks.TopTen();
            
            Jwt? jwt = _db.VerifyJwt();
            if(jwt != null)
            {
                ViewData["Jwt"] = jwt.Token;
                return View(topTen);
            }

          return View(topTen);


        }

       
        public IActionResult Privacy()
        {

            Jwt? jwt = _db.VerifyJwt();
            if (jwt != null)
            {
                ViewData["Jwt"] = jwt.Token;
                return View();
            }

            return RedirectToAction("Index", "Auth"); 
            
           
            
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}