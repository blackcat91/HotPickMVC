using HotPickMVC.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace HotPickMVC.Data.Portfolios
{
    public class PortfolioData : IPortfolioData
    {
        private readonly MongoDBAccess _db;
        private readonly IConfiguration _config;
        private const string PortfoliosCollection = "portfolios";

        public PortfolioData(MongoDBAccess db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public async Task<List<PortfolioModel>> getPortfolios(string? user)
        {
            var getPort = _db.ConnectToMongo<PortfolioModel>(PortfoliosCollection);
            var portfolios = new List<PortfolioModel>();
            var portfolio = new PortfolioModel();

            if (user == null)
            {
                portfolios = await getPort.Find(p => p.IsPublic == true).ToListAsync();
                return portfolios;
            }

            portfolios = await getPort.Find(p => p.UserId == user).ToListAsync();

            return portfolios;


        }

        public async Task EditPortfolio(PortfolioModel portfolio)
        {
            var getPort = _db.ConnectToMongo<PortfolioModel>(PortfoliosCollection);
            var filter = Builders<PortfolioModel>.Filter.Eq("Id", portfolio.Id);
            var port = await getPort.Find(filter).FirstAsync();
            port.Name = portfolio.Name;
            await getPort.ReplaceOneAsync(filter, port);

        }

        public async Task CreatePortfolio(PortfolioModel portfolio)
        {
            var client = new MongoClient(_config.GetConnectionString("Default"));
            using var session = await client.StartSessionAsync();
            session.StartTransaction();

            try
            {
                var db = client.GetDatabase("hotpick");
                var getPort = db.GetCollection<PortfolioModel>("portfolios");
                await getPort.InsertOneAsync(portfolio);
                var user = db.GetCollection<UserModel>("users");
                var userD = user.Find(u => u.Id == portfolio.UserId).FirstOrDefault();
                userD.Portfolios!.Add(portfolio.Id);
                var filter = Builders<UserModel>.Filter.Eq("Id", userD.Id);

                await user.ReplaceOneAsync(filter, userD);

                await session.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await session.AbortTransactionAsync();
                Console.WriteLine(ex.Message);
            }

        }
        public async Task DeletePortfolio(PortfolioModel portfolio)
        {
            var client = new MongoClient(_config.GetConnectionString("Default"));
            using var session = await client.StartSessionAsync();
            session.StartTransaction();

            try
            {
                var db = client.GetDatabase("hotpick");
                var getPort = db.GetCollection<PortfolioModel>("portfolios");
                var filter = Builders<PortfolioModel>.Filter.Eq("Id", portfolio.Id);
                await getPort.DeleteOneAsync(filter);
                var user = db.GetCollection<UserModel>("users");
                var userD = user.Find(u => u.Id == portfolio.UserId).FirstOrDefault();
                userD.Portfolios!.Remove(portfolio.Id);
                var ufilter = Builders<UserModel>.Filter.Eq("Id", userD.Id);

                await user.ReplaceOneAsync(ufilter, userD);

                await session.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await session.AbortTransactionAsync();
                Console.WriteLine(ex.Message);
            }

        }


        public async Task AddStock(string pId, StockModel ticker)
        {
            var portfolioCollection = _db.ConnectToMongo<PortfolioModel>(PortfoliosCollection);
            var portfolio = await portfolioCollection.Find(u => u.Id == pId).FirstOrDefaultAsync();
            var filter = Builders<PortfolioModel>.Filter.Eq("Id", pId);
            if (portfolio.Stocks!.Exists(s => s.ticker == ticker.ticker) == false)
            {
                portfolio.Stocks.Add(ticker);
                await portfolioCollection.ReplaceOneAsync(filter, portfolio);
                return;
            }


            throw new InvalidOperationException("Already Contained!");

        }

        public async Task RemoveStock(string pId, StockModel ticker)
        {
            var portfolioCollection = _db.ConnectToMongo<PortfolioModel>(PortfoliosCollection);
            var portfolio = await portfolioCollection.Find(u => u.Id == pId).FirstOrDefaultAsync();
            var filter = Builders<PortfolioModel>.Filter.Eq("Id", pId);
            if (portfolio.Stocks!.Exists(s => s.ticker == ticker.ticker))
            {
                portfolio.Stocks.Remove(portfolio.Stocks!.Find(s => s.ticker == ticker.ticker));
                await portfolioCollection.ReplaceOneAsync(filter, portfolio);
                return;
            }


            throw new InvalidOperationException("Not Contained!");

        }


    }
}
