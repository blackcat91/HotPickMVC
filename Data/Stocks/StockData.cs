using HotPickMVC.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace HotPickMVC.Data.Stocks
{
    public class StockData : IStockData
    {
        private readonly MongoDBAccess _db;
        private const string StocksCollection = "stocks";

        public StockData(MongoDBAccess db)
        {
            _db = db;
        }

        public async Task<List<StockModel>?> GetStock(string tickers)
        {
            var stocks = _db.ConnectToMongo<StockModel>(StocksCollection);


            try
            {
                if (tickers != null)
                {
                    var filter = Builders<StockModel>.Filter.Eq("ticker", tickers);
                    var results = (await stocks.FindAsync(filter)).ToList();
                    foreach (var stock in results)
                    {
                        stock.overall = Math.Round(stock.overall * 100, 2);
                    }
                    return results;

                }

                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                return null;
            }

        }



        public async Task<List<StockModel>?> SearchStocks(string query)
        {
            var stocks = _db.ConnectToMongo<StockModel>(StocksCollection);
            var filter = Builders<StockModel>.Filter.Regex("ticker", new BsonRegularExpression(query.ToUpper(), "i")) | Builders<StockModel>.Filter.Regex("company", new BsonRegularExpression(query.ToUpper(), "i"));

            try
            {
                var results = stocks.Find(filter);
                var r = await results.ToListAsync();
                foreach (var stock in r)
                {
                    stock.overall = Math.Round(stock.overall * 100, 2);
                }
                return r;
            }
            catch (Exception ex)
            {

                return null;
            }

        }


        public async Task<List<StockModel>?> PaginateStocks(int? pageNum)
        {
            var stocks = _db.ConnectToMongo<StockModel>(StocksCollection);
            List<StockModel> results = new List<StockModel> { };

            try
            {
                results = await stocks.Find(_ => true).SortBy(s => s.ticker).Skip(16 * (pageNum - 1)).Limit(16).ToListAsync();
                foreach (var stock in results)
                {
                    stock.overall = Math.Round(stock.overall * 100, 2) ;
                }

                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        public async Task<List<StockModel>?> PaginateStocksPrev(string? startId)
        {
            var stocks = _db.ConnectToMongo<StockModel>(StocksCollection);
            List<StockModel> results = new List<StockModel> { };

            try
            {
                if (startId == null)
                {
                    results = await stocks.Find(_ => true).SortBy(s => s.ticker).Limit(16).ToListAsync();
                }
                else
                {
                    var filter = Builders<StockModel>.Filter.AnyLt("Id", startId);
                    results = await stocks.Find(filter).SortBy(s => s.ticker).Limit(16).ToListAsync();
                }
                
                return results;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<List<StockModel>> TopTen()
        {
            var stocks = _db.ConnectToMongo<StockModel>(StocksCollection);
            var topT = await stocks.Find(_ => true).SortByDescending(s => s.overall).Limit(10).ToListAsync();
            foreach(var stock in topT)
            {
                stock.overall = Math.Round(stock.overall * 100, 2);
            }
            return topT;
        }
    }
}
