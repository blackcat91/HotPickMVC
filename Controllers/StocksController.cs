using HotPickMVC.Data;
using HotPickMVC.Data.Portfolios;
using HotPickMVC.Data.Stocks;
using HotPickMVC.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Web.Helpers;

namespace HotPickMVC.Controllers
{
    public class StocksController : Controller
    {

        private readonly IStockData _stocks;
        private readonly IPortfolioData _port;
        private readonly MongoDBAccess _db;
        private readonly ThirdPartyData _tbd;

        public StocksController(IPortfolioData port, IStockData stocks , MongoDBAccess db, ThirdPartyData tbd)
        {
            _db = db;
            _tbd = tbd;
            _stocks = stocks;
            _port = port;
        }

        public async Task<IActionResult> Index(string? id)
        {
            var stockControl = _db.ConnectToMongo<StockModel>("stocks");
            var maxpages = Math.Round((decimal)(stockControl.Find(_ => true).CountDocuments() / 15), 0);
            maxpages = (stockControl.Find(_ => true).CountDocuments() % 15) > 0 ? maxpages + 1 : maxpages;
            ViewData["maxPages"] = maxpages;
            int numid = Convert.ToInt16(id);
            List<StockModel> stocks = new List<StockModel> { };
            
            
            if (id == null || numid == 1)
            {
                ViewData["page"] = 1;
                ViewData["next"] = (Int32)ViewData["page"] + 1;
               
                stocks = await _stocks.PaginateStocks(1);
            }
            else
            {
                
                ViewData["page"] = numid;
                
                ViewData["next"] = (Int32)ViewData["page"] + 1;
                ViewData["prev"] = (Int32)ViewData["page"] - 1;
                if (maxpages < numid)
                {
                    ViewData["next"] = maxpages + 1;
                    ViewData["prev"] = maxpages - 1;
                    return View(await _stocks.PaginateStocks((int)maxpages));
                }
                stocks = await _stocks.PaginateStocks(numid);
            }

            
            Jwt? jwt = _db.VerifyJwt();
            
            if (jwt != null)
            {
                ViewData["Jwt"] = jwt.Token;
                return View(stocks);
            }

            return View(stocks);
        }

        
        public async Task<IActionResult> GetStock(string ticker)
        {
            var stockStream = await _tbd.GetTickerStream(ticker);

            if (stockStream!.Count == 0) return RedirectToAction("Index", "Stocks" ,null);
            
            var newStream = await _tbd.GetNewsStream(ticker);
            if (!newStream!.Items.Any()) return RedirectToAction("Index");
            Jwt? jwt = _db.VerifyJwt();
            StockModel? stock = (await _stocks.GetStock(ticker))!.First();
            var stockInfo = new AllStockDataModel(stockStream!, stock!);
            stockInfo.News = newStream!.Items.ToList();
            
            if (jwt != null)
            {
                ViewData["Jwt"] = jwt.Token;
                stockInfo.Portfolios = await _port.getPortfolios(jwt.Id);
                 
                return View(stockInfo);
            }
            
            return View(stockInfo);
        }

        [HttpPost]
        public async Task<IActionResult> Search(string query)
        {
            var results = await _stocks.SearchStocks(query);
           
            return View(results);
        }


        [HttpPost]
        public async Task<dynamic> AddStock(string stock, string portfolio)
        {

            try
            {
                var stoc = (await _stocks.GetStock(stock))!.First();
                await _port.AddStock(portfolio, stoc);
                return true;
            }
            catch(Exception e)
            {
                return e.Message;
            }
           
        }

        [HttpPost]
        public async Task<dynamic> RemoveStock(string stock, string portfolio)
        {

            try
            {
                var stoc = (await _stocks.GetStock(stock))!.First();
                await _port.RemoveStock(portfolio, stoc);
                return true;
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }


    }
}
